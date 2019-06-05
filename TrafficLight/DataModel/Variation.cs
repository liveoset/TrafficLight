using System;
using LiteDB;

namespace TestTrafficLight.DataModel
{
    /// <summary>
    /// Возможный вариант начала отсчета для Sequence
    /// </summary>
    public class Variation
    {
        public ObjectId Id { get; set; }
        /// <summary>
        /// Guid последовательности
        /// </summary>
        public Guid Sequence { get; set; }
        /// <summary>
        /// Стартовое значение
        /// </summary>
        public byte Start { get; set; }
        /// <summary>
        /// Текущее значение, с учетом старта
        /// </summary>
        public byte Current { get; set; }
        /// <summary>
        /// Статус варианта. false - вариант невозможен. true - возможный вариант
        /// </summary>
        public bool Status { get; set; }
    }
}