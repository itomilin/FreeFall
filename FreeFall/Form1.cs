/**
 * Составить программу, которая моделирует свободное падение тела с учётом сопротивления среды.
 * В программе использовать визуализацию.
 */


using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;

namespace FreeFall
{
    public partial class Form1 : Form
    {
        private double travellingDistance = 0; // Пройденное расстояние телом.
        private double x = 0;                  // 
        private const double G = 9.81;         // Ускорение свободного падения.
        private const double C = 0.4;          // Коэффициент сопротивления от формы тела(шар).
        private double V = 0;                  // Скорость.
        private double step = 0;               // Шаг моделирования.
        private double _mg = 0;                // Произведение (для подстановки)
        private double _s = 0;                 // Площадь поперечного сечения.
        private double _height = 0;            // Высота тела.
        private double _weight = 0;            // Вес тела.
        private double _k1 = 0;                // Коэффициент сопротивления.
        private double _k2 = 0;                // Коэффициент сопротивления.
        private double _viscosity = 0;         // Вязкость среды.
        private double _density = 0;           // Плотность среды.
        private double k1, k2, k3, k4;

        private Graphics rg;
        // Начальные положения и радиус. Окружность строится через прямоугольник.
        private RectangleF rectangle = new RectangleF(0, 0, 40, 40);

        
        public Form1()
        {
            InitializeComponent();
            comboBox1.Items.AddRange(ModelingData.GetProps().Keys.ToArray());
        }


        /// <summary>
        /// Метод, запускает процесс моделирования.
        /// В нем происходит задание начальных величин из полей ввода.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            // Обнуляем все величины
            pictureBox1.InitialImage = null;
            rectangle.Y = 0F; // Устанавливаем положение

            travellingDistance = 0;
            V = 0.0;
            step = (double)numericUpDown1.Value;
            x = 0;

            try
            {
                _weight = (float)Convert.ToDouble(tBWeight.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Введите вес!",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            if (comboBox1.SelectedItem != null)
            {
                _viscosity = ModelingData.GetProps().Where(x => x.Key
                .Contains(comboBox1.SelectedItem.ToString())).First().Value.Item1;
                _density = ModelingData.GetProps().Where(x => x.Key
                .Contains(comboBox1.SelectedItem.ToString())).First().Value.Item2;
            }
            else
            {
                MessageBox.Show("Выберите среду!",
                                   "Error",
                                   MessageBoxButtons.OK,
                                   MessageBoxIcon.Error);
                return;
            }

            try
            {
                _height = (float)Convert.ToDouble(tBHeight.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Введите высоту!",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            // Поперечное сечение или площадь? 
            _s = Math.PI * Math.Pow(rectangle.Height / 2, 1);
            // Коэффициент сопротивления
            _k1 = 6 * Math.PI * _viscosity * (rectangle.Height / 2);
            // Коэффициент сопротивления
            _k2 = 0.5 * C * _density * _s;

            // Для удобности, вес * ускорение падения
            _mg = G * _weight;

            timer1.Start();
            button1.Enabled = false;
        }


        // Функция, описывающая падение тела.
        public double Function(double x, double y)
        {
            return (_mg - (_k1 * y) - _k2 * Math.Pow(y, 2)) / _weight;
        }


        /// <summary>
        /// С помощью таймера, моделируем падение шара.
        /// Отрисовка примерно 40 кадров в секунду.
        /// Задается в свойствах таймера в мс (1 сек - 1000мс).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            tBDistance.Text = travellingDistance.ToString();
            tBSpeed.Text = V.ToString();
            tBTime.Text = x.ToString();
            rectangle.Y += (float)V;
            travellingDistance += V * step;


            // Метод Эйлера
            //V += step * (_mg - (_k1 * V) - (_k2 * Math.Pow(V, 2))) / _weight;
            
            // Метод Рунге-Кутта 4 степени
            k1 = Function(x, V);
            k2 = Function(x + (step * 1/2), V + (step * k1 * 1/2));
            k3 = Function(x + (step * 1/2), V + (step * k2 * 1/2));
            k4 = Function(x + step, V + (step * k3));
            V += (step / 6.0) * (k1 + 2 * k2 + 2 * k3 + k4); 
            //

            // Перерисовываем контрол с шаром.
            pictureBox1.Invalidate();

            // Проверяем пройденное расстояние, которое указали на форме.
            // Условием выхода, является достижение указанной высоты.
            if (travellingDistance >= _height)
            {
                button1.Enabled = true;
                timer1.Stop();
            }
            else if (V <= 0) // Значит тело не имеет скорости (не тонет)
            {
                button1.Enabled = true;
                timer1.Stop();
            }
            // Если шар вышел за границы, возвращаем на 0 координату
            if (rectangle.Y + rectangle.Height > pictureBox1.Height)
                rectangle.Y = 0;

            x += step; // Увеличиваем аргумент функции на заданный шаг, он же является временем.
        }


        /// <summary>
        /// Метод отрисовки шара.
        /// Выполняем отрисовку шара в pictureBox.
        /// Задаем начальное положение.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            rectangle.X = pictureBox1.Width / 2; // Устанавливаем положение
            rg = e.Graphics;
            rg.FillEllipse(Brushes.Red, rectangle);
            rg.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        }


        /// <summary>
        /// Метод остановки моделирования.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            button1.Enabled = true;
        }
    }
}