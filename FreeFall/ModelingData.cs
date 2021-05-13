using System;
using System.Collections.Generic;

namespace FreeFall
{
    static public class ModelingData
    {
        /// <summary>
        /// Свойства для разных сред. Оформлено в виде словаря.
        /// Ключом указана среда.
        /// Значения:
        /// Вязкость указана первым параметром кортежа.
        /// Плотность указана вторым параметром кортежа.
        /// </summary>
        private static readonly Dictionary<string, Tuple<double, double>> _props =
            new Dictionary<string, Tuple<double, double>>()
            {
                ["Гелий"] = new Tuple<double, double>(2.0 * Math.Pow(10, -5), 0.178),
                ["Вода"] = new Tuple<double, double>(8.9 * Math.Pow(10, -4), 1000),
                ["Воздух"] = new Tuple<double, double>(1.78 * Math.Pow(10, -5), 1.293),
                ["Глицерин"] = new Tuple<double, double>(1.490, 1260)
            };


        /// <summary>
        /// Свойство, возвращаем словарь с данными.
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, Tuple<double, double>> GetProps()
        {
            return _props;
        }
    }
}
