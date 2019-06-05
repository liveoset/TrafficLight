using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Hosting;
using LiteDB;
using TestTrafficLight.DataModel;

namespace TestTrafficLight.Models
{
    /// <summary>
    /// Класс основной логики решения задачи
    /// </summary>
    public class TaskSolver
    {
        private static readonly TaskSolver _instance;
        public static TaskSolver Instance => _instance;

        static TaskSolver()
        {
            _instance = new TaskSolver();
        }

        private LiteDatabase Connection { get; set; }
        private readonly byte[] DigitsInv;
        private readonly byte[] Digits;
        public TaskSolver()
        {
            //Получаем строку подключения к БД
            var connectionString = HostingEnvironment.MapPath("/db/") + "app.db";
            if (ConfigurationManager.ConnectionStrings["LiteDB"] != null)
            {
                connectionString = ConfigurationManager.ConnectionStrings["LiteDB"].ConnectionString;
            }

            Connection = new LiteDatabase(connectionString);
            //Создаем таблицы, если их нет.
            //Добавляем в них индексы
            var items = Connection.GetCollection<Item>("items");
            items.EnsureIndex(x => x.Sequence);
            var variations = Connection.GetCollection<Variation>("variations");
            variations.EnsureIndex(x => x.Sequence);
            variations.EnsureIndex(x => x.Status);
            var broken = Connection.GetCollection<Broken>("broken");
            broken.EnsureIndex(x => x.Sequence);

            //Константы битовых масок
            Digits = new byte[10]
            {
                119,    //0
                18,     //1
                93,     //2
                91,     //3
                58,     //4
                107,    //5
                111,    //6
                82,     //7
                127,    //8
                123,    //9
            };
            DigitsInv = new byte[10]
            {
                8,   //0
                109, //1
                34,  //2
                36,  //3
                69,  //4
                20,  //5
                16,  //6
                45,  //7
                0,   //8
                4,    //9
            };
        }

        /// <summary>
        /// Иницировать процесс съема наблюдений
        /// </summary>
        /// <returns>Токен последовательности наблюдений</returns>
        public Guid Create()
        {
            //Генерируем новый сикванс и добавляем в базу под номером 0.
            var newSequence = Guid.NewGuid();
            var items = Connection.GetCollection<Item>("items");

            items.Insert(new Item
            {
                ItemNo = 0,
                Sequence = newSequence,
            });


            //Заполним всеми возможными Вариантами
            //Их всего 99 [1..99]
            var variations = Connection.GetCollection<Variation>("variations");
            var list = new List<Variation>();
            for (byte i = 1; i < 100; i++)
            {
                list.Add(new Variation()
                {
                    Status = true,
                    Current = i,
                    Start = i,
                    Sequence = newSequence
                });
            }
            variations.InsertBulk(list);

            //Таблица нерабочих секций. Предположительно все работают, поэтому добавляем 0
            var broken = Connection.GetCollection<Broken>("broken");
            broken.Insert(new Broken()
            {
                Sequence = newSequence,
                Value1 = 0,
                Value2 = 0
            });
            return newSequence;
        }
        /// <summary>
        /// Добавить очередное наблюдение
        /// </summary>
        /// <param name="request">Запрос с параметрами наблюдения</param>
        /// <returns>Результат</returns>
        public BaseResponse Add(ObservationRequest request)
        {
            //Проверка входных данных

            var items = Connection.GetCollection<Item>("items");


            var observations = items.Find(r => r.Sequence == request.sequence).OrderBy(r => r.ItemNo).ToList();
            if (!observations.Any()) return new ErrorResponse("The sequence isn't found");
            if (request.observation == null)
            {
                return new ErrorResponse("The field 'observation' required!");
            }

            if (request.observation.color != "red" && request.observation.color != "green")
            {
                return new ErrorResponse("The field 'color' has to be red or green");
            }

            if (request.observation.color == "green")
            {
                if (request.observation.numbers == null) return new ErrorResponse("The field 'numbers' required!");
                if (request.observation.numbers.Length != 2)
                {
                    return new ErrorResponse("The field 'numbers' must contaion exactly 2 elements");
                }
                if (!Tools.IdValidObservation(request.observation.numbers[0]))
                    return new ErrorResponse("The first item of the field 'numbers' has invalid format");
                if (!Tools.IdValidObservation(request.observation.numbers[1]))
                    return new ErrorResponse("The second item of the field 'numbers' has invalid format");

                if (observations.Last().Red) return new ErrorResponse("The red observation should be the last");

            }
            else
            {
                if (observations.Count() == 1) return new ErrorResponse("There isn't enough data");


            }
            var num = observations.Last().ItemNo;
            var ni = new Item()
            {
                Sequence = request.sequence,
                ItemNo = num + 1,
                //Number1 =request.observation.numbers?[0],
                //Number2 = request.observation.numbers?[1],
                Digit1 = request.observation.numbers?[0].ConvertToByte() ?? (byte)0,
                Digit2 = request.observation.numbers?[1].ConvertToByte() ?? (byte)0,
                Red = request.observation.color == "red"
            };
            items.Insert(ni);
            observations.Add(ni);

            //Основная логика

            var variations = Connection.GetCollection<Variation>("variations");
            var varList = variations.Find(r => r.Sequence == request.sequence && r.Status).ToList();
            var broken = Connection.GetCollection<Broken>("broken");
            var br = broken.FindOne(x => x.Sequence == request.sequence);
            var resList = new List<byte>();


            //Если не первый замер то декримируем число во всех кобинациях
            //А там, где стало меньше 0, удаляем, так как это означает, что такой комбинации быть не должно
            if (num > 0)
            {
                foreach (var s in varList)
                {
                    if (s.Current == 0)
                        s.Status = false;
                    else
                        s.Current--;
                }
            }

            //В случае, если красные сингал, то мы уперлись в конец.
            //При этом если есть хоть одна комбинация с нулевым текущим значением, то это наше решение.
            //все остальное удалить.
            if (ni.Red)
            {
                foreach (var s in varList)
                {
                    if (!s.Status) continue;
                    if (s.Current != 0) s.Status = false;

                }

            }
            else
            {
                //На зеленом сигнале удаляем Варианты, которые противоречат принятому образу
                foreach (var s in varList)
                {
                    var d1 = (byte)(s.Current / 10);
                    var d2 = (byte)(s.Current % 10);

                    if ((DigitsInv[d1] & ni.Digit1) != 0 || (DigitsInv[d2] & ni.Digit2) != 0)
                        s.Status = false;

                }

            }

            // Обрабатываем нерабочие секции
            // Принцип такой: мы бежим по истории назад, для оставшихся комбинаций, и смотрим их исторические значения,
            // сверяя с историческими образами.
            // Если секция обязательна для всех оставшихся комбинаций, но не горит, то это означает, что она не работает.
            for (var i = (byte)(observations.Count - 1); i > 0; i--)
            {
                var obs = observations[i];
                if (obs.Red) continue;

                var req1 = obs.Digit1.Invert7();
                var req2 = obs.Digit2.Invert7();
                foreach (var s in varList)
                {
                    if (!s.Status) continue;
                    var d1 = (byte)((s.Start - i + 1) / 10);
                    var d2 = (byte)((s.Start - i + 1) % 10);
                    req1 = (byte)(req1 & Digits[d1]);
                    req2 = (byte)(req2 & Digits[d2]);
                }

                br.Value1 = (byte)(br.Value1 | req1);
                br.Value2 = (byte)(br.Value2 | req2);
            }


            //Формируем массив ответа
            foreach (var s in varList)
            {
                if (s.Status) resList.Add(s.Start);
            }
            //Записываем в базу изменени
            variations.Update(varList);
            broken.Update(br);
            //Если комбинаций нет, то значит имеется какое-то противоречие, которое обнулило все комбинации.
            if (resList.Count == 0) return new ErrorResponse("No solutions found");
            return new ObservationResponse(resList, br.Value1, br.Value2);


        }

        /// <summary>
        /// Очистить базу данных
        /// </summary>
        /// <returns>Результат очистки</returns>
        public ClearResponse Clear()
        {
            Connection.DropCollection("items");
            Connection.DropCollection("variations");
            Connection.DropCollection("broken");

            return new ClearResponse();
        }

        /// <summary>
        /// Показывает содержимое переданной таблицы базы данных
        /// </summary>
        /// <param name="tableName">Название таблицы</param>
        /// <returns>Список записей</returns>
        public IList Show(string tableName)
        {
            switch (tableName)
            {
                case "items":
                    return Connection.GetCollection<Item>("items").FindAll().ToList();
                case "variations":
                    return Connection.GetCollection<Variation>("variations").FindAll().ToList();
                case "broken":
                    return Connection.GetCollection<Broken>("broken").FindAll().ToList();

            }

            return null;

        }

        /// <summary>
        /// Вспомогательная функция для получения Образов наблюдения из числа и масок сломанных секций
        /// </summary>
        /// <param name="value">Наблюдаемое число</param>
        /// <param name="mask1">Маска первой цифры bit 1 - ячейка рабочая,bit 0 - не рабочая</param>
        /// <param name="mask2">Маска второй цифры bit 1 - ячейка рабочая,bit 0 - не рабочая</param>
        /// <returns></returns>
        public string[] EmulateBrokenDigits(byte value, byte mask1, byte mask2)
        {

            var v1 = (byte)(value / 10);
            var v2 = (byte)(value - v1 * 10);
            var r1 = Digits[v1];
            var r2 = Digits[v2];
            var r = new string[2];
            r[0] = ((byte)(r1 & mask1)).ConvertToBString();
            r[1] = ((byte)(r2 & mask2)).ConvertToBString();
            return r;
        }

    }
}