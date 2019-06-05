using System;

namespace TestTrafficLight.Models
{
    /// <summary>
    /// Класс для вспомогательных инструментов
    /// </summary>
    public static class Tools
    {
        /// <summary>
        /// Конвертирует переданный byte в 7значную строку битов с лидирующими нулями  
        /// </summary>
        /// <param name="bt"></param>
        /// <returns></returns>
        public static string ConvertToBString(this byte  bt)
        {
            return Convert.ToString(bt, 2).PadLeft(7, '0');
        }

        /// <summary>
        /// Конвертирует переданную строку в 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte ConvertToByte(this string str)
        {
            return Convert.ToByte(str, 2);

        }

        /// <summary>
        /// Данная функция инвертирует 7 битов справа введенного байта
        /// </summary>
        /// <param name="b">Байт</param>
        /// <returns></returns>
        public static byte Invert7(this byte b)
        {
            b = (byte)~b;
            b = (byte)(b & 127);
            return b;
        }

        /// <summary>
        /// Проверка строки на соответвие шаблону
        /// Она должна быть по длине ровно 7 символов и содержать только 0 и 1
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IdValidObservation(string str)
        {
            if (str == null) return false;
            if (str.Length != 7) return false;
            foreach (var s in str)
                if (s != '1' && s != '0')
                    return false;

            return true;
        }
        
    }
}