using System;
using LiteDB;

namespace TestTrafficLight.DataModel
{
    /// <summary>
    /// Маска сломанных ячеек для Sequence
    /// </summary>
    public class Broken
    {
        public ObjectId Id { get; set; }
        /// <summary>
        /// Guid последовательности
        /// </summary>
        public Guid Sequence { get; set; }
        /// <summary>
        /// Маска первой цифры
        /// </summary>
        public byte Value1 { get; set; }
        /// <summary>
        /// Маска второй цифры
        /// </summary>
        public byte Value2 { get; set; }
    }
}