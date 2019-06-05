using System;
using LiteDB;

namespace TestTrafficLight.DataModel
{
    /// <summary>
    /// Запись истории наблюдений
    /// 
    /// </summary>
    public class Item
    {
        public ObjectId Id { get; set; }
        /// <summary>
        /// Guid последовательности
        /// </summary>
        public Guid Sequence { get; set; }
        /// <summary>
        /// Номер наблюдения. 0 - это вспомогательная запись, содержит только Sequence
        /// Наблюдения начинаются с 1.
        /// </summary>
        public int ItemNo { get; set; }
        /// <summary>
        /// Значение первой цифры
        /// </summary>
        public byte Digit1 { get;set; }
        /// <summary>
        /// Значение второй цифры
        /// </summary>
        public byte Digit2 { get;set; }
        /// <summary>
        /// Цвет светофора true - красный, false - зеленый
        /// </summary>
        public bool Red { get; set; }
    }
}