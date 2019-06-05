using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestTrafficLight.Controllers;
using TestTrafficLight.Models;

namespace TestTraficLight.Controllers.Tests
{
    [TestClass]
    public class MainControllerTests
    {
        [TestMethod]
        public void TestCreate()
        {
            var controller = new MainController();
            var res = controller.Create();
            Assert.AreEqual(res.status, "ok");
            Assert.AreNotEqual(res.response.sequence,Guid.Empty);
        }
        [TestMethod]
        public void TestClear()
        {
            var controller = new MainController();
            var res = controller.Clear();
            Assert.AreEqual(res.status, "ok");
            Assert.AreEqual(controller.Show("items").Count,0);
            Assert.AreEqual(controller.Show("variations").Count,0);
            Assert.AreEqual(controller.Show("broken").Count,0);
            
        }
        [TestMethod]
        public void TestCase1()
        {

            //Пример из задачи
            var controller = new MainController();


            var res = controller.Create();
            Assert.AreEqual(res.status, "ok");

            var res2 = controller.Add(new ObservationRequest
            {
                sequence = res.response.sequence,
                observation = new Observation
                {
                    color = "green",
                    numbers = new string[2] { "1110111", "0011101" }
                }
            });
            Assert.IsInstanceOfType(res2, typeof(ObservationResponse));
            var res3 = res2 as ObservationResponse;
            Assert.IsTrue(res3.numbers.SequenceEqual(new List<byte> { 2, 8, 82, 88 }));

            var res4 = controller.Add(new ObservationRequest
            {
                sequence = res.response.sequence,
                observation = new Observation
                {
                    color = "green",
                    numbers = new string[2] { "1110111", "0010000" },
                }
            });

            Assert.IsInstanceOfType(res4, typeof(ObservationResponse));
            var res5 = res4 as ObservationResponse;
            Assert.IsTrue(res5.numbers.SequenceEqual(new List<byte> { 2, 8, 82, 88 }));

            var res6 = controller.Add(new ObservationRequest
            {
                sequence = res.response.sequence,
                observation = new Observation
                {
                    color = "red"
                }
            });

            Assert.IsInstanceOfType(res6, typeof(ObservationResponse));
            var res7 = res6 as ObservationResponse;
            Assert.IsTrue(res7.numbers.SequenceEqual(new List<byte> { 2 }));

            //Плюс проверим The red observation should be the last
            var res8 = controller.Add(new ObservationRequest
            {
                sequence = res.response.sequence,
                observation = new Observation
                {
                    color = "green",
                    numbers = new string[2] { "1110111", "0010000" },
                }
            });

            Assert.IsInstanceOfType(res8, typeof(ErrorResponse));
            Assert.IsTrue(((ErrorResponse)res8).msg == "The red observation should be the last");


        }
        [TestMethod]
        public void TestCaseValidation()
        {
            var controller = new MainController();

            //The sequence isn't found
            var res1 = controller.Add(new ObservationRequest
            {
                sequence = Guid.NewGuid(),
                observation = new Observation
                {
                    color = "green",
                    numbers = new string[2] { "1110111", "0011101" }
                }
            });

            Assert.IsTrue(res1.status == "error");
            Assert.IsTrue(((ErrorResponse)res1).msg == "The sequence isn't found");
            //There isn't enough data
            var res = controller.Create();
            Assert.AreEqual(res.status, "ok");
            var res2 = controller.Add(new ObservationRequest
            {
                sequence = res.response.sequence,
                observation = new Observation
                {
                    color = "red"
                }
            });

            Assert.IsTrue(res2.status == "error");
            Assert.AreEqual(((ErrorResponse)res2).msg, "There isn't enough data");


            //The field 'observation' required!
            var res3 = controller.Create();
            Assert.AreEqual(res3.status, "ok");
            var res4 = controller.Add(new ObservationRequest
            {
                sequence = res3.response.sequence,
            });

            Assert.IsTrue(res4.status == "error");
            Assert.AreEqual(((ErrorResponse)res4).msg, "The field 'observation' required!");

            //The field 'color' has to be red or green
            var res5 = controller.Create();
            Assert.AreEqual(res3.status, "ok");
            var res6 = controller.Add(new ObservationRequest
            {
                sequence = res5.response.sequence,
                observation = new Observation() {color = "blue"}
            });

            Assert.IsTrue(res6.status == "error");
            Assert.AreEqual(((ErrorResponse)res6).msg, "The field 'color' has to be red or green");

            //The field 'numbers' must contaion exactly 2 elements
            var res7 = controller.Create();
            Assert.AreEqual(res3.status, "ok");
            var res8 = controller.Add(new ObservationRequest
            {
                sequence = res7.response.sequence,
                observation = new Observation() {color = "green",numbers = new string[0]}
            });

            Assert.IsTrue(res8.status == "error");
            Assert.AreEqual(((ErrorResponse)res8).msg, "The field 'numbers' must contaion exactly 2 elements");

            //The first item of the field 'numbers' has invalid format
            var res9 = controller.Create();
            Assert.AreEqual(res3.status, "ok");
            var res10 = controller.Add(new ObservationRequest
            {
                sequence = res9.response.sequence,
                observation = new Observation() {color = "green",numbers = new string[2] {"12","1111111"}}
            });

            Assert.IsTrue(res10.status == "error");
            Assert.AreEqual(((ErrorResponse)res10).msg, "The first item of the field 'numbers' has invalid format");

            //The second item of the field 'numbers' has invalid format
            var res11 = controller.Create();
            Assert.AreEqual(res3.status, "ok");
            var res12 = controller.Add(new ObservationRequest
            {
                sequence = res11.response.sequence,
                observation = new Observation() {color = "green",numbers = new string[2] {"1100001","111011"}}
            });

            Assert.IsTrue(res12.status == "error");
            Assert.AreEqual(((ErrorResponse)res12).msg, "The second item of the field 'numbers' has invalid format");

        }
        [TestMethod]
        public void TestCase3()
        {

            //No solutions found
            var controller = new MainController();


            var res = controller.Create();
            Assert.AreEqual(res.status, "ok");

            var res2 = controller.Add(new ObservationRequest
            {
                sequence = res.response.sequence,
                observation = new Observation
                {
                    color = "green",
                    numbers = new string[2] { "1110111", "0011101" }
                }
            });

            Assert.IsInstanceOfType(res2, typeof(ObservationResponse));
            var res3 = res2 as ObservationResponse;
            Assert.IsTrue(res3.numbers.SequenceEqual(new List<byte> { 2, 8, 82, 88 }));

            var res4 = controller.Add(new ObservationRequest
            {
                sequence = res.response.sequence,
                observation = new Observation
                {
                    color = "green",
                    numbers = new string[2] { "1110111", "0011101" }
                }
            });

            Assert.IsInstanceOfType(res4, typeof(ErrorResponse));
            Assert.AreEqual(((ErrorResponse)res4).msg, "No solutions found");




        }

        [TestMethod]
        public void TestCaseRandom()
        {
            //Проверка на 200 случайных значениях. Процесс длится  58 сек
            var rnd = new Random();
            var controller = new MainController();
            for (var i = 1; i < 200; i++)
            {
                //Определяем случайное стартовое значение
                byte start = (byte)rnd.Next(1, 99);
                //Определям случайные поломанные секции
                byte mask1 = (byte)rnd.Next(1, 127); //127;
                byte mask2 = (byte)rnd.Next(1, 127); //127;


                var res = controller.Create();
                Assert.AreEqual(res.status, "ok");
                //наблюдатель начинает со старта передавать данные в сервис
                //По сути это обратный отсчет с проедением его через маску поломанных секций.
                for (byte n = start; n > 0; n--)
                {
                    var res2 = controller.Add(new ObservationRequest
                    {
                        sequence = res.response.sequence,
                        observation = new Observation
                        {
                            color = "green",
                            numbers = controller.EmulateBrokenDigits(n, mask1, mask2)
                        }
                    });
                    Assert.IsInstanceOfType(res2, typeof(ObservationResponse));
                    var res3 = res2 as ObservationResponse;
                    //Если осталось одно значение, проверим, оно ли это
                    if (res3.numbers.Count == 1)
                    {
                        Assert.AreEqual(res3.numbers[0], start);
                    }
                }

                var res4 = controller.Add(new ObservationRequest
                {
                    sequence = res.response.sequence,
                    observation = new Observation
                    {
                        color = "red",
                    }
                });
                Assert.IsInstanceOfType(res4, typeof(ObservationResponse));
                var res5=(ObservationResponse)res4;
                Assert.AreEqual(res5.numbers[0], start);
                //При старте менее 20, не всегда находит точно все не рабочие ячейки, поэтому проверям, только если больше
                if (start > 19)
                {
                    Assert.AreEqual(res5.missing[0],mask1.Invert7().ConvertToBString());
                    Assert.AreEqual(res5.missing[1],mask2.Invert7().ConvertToBString());
                }

                
               
                

            }




        }

        [TestMethod]
        public void TestCaseExtrim()
        {
            //Тут проведм тестирование на краевых моментах
            var tests = new byte[,]
            {
                {99, 127,127},
                {99, 127,0},
                {99, 0,127},
                {99, 0,0},
                {50, 127,127},
                {50, 127,0},
                {50, 0,127},
                {50, 0,0},
                {1, 127,127},
                {1, 127,0},
                {1, 0,127},
                {1, 0,0},
                
                
            };
            
            var controller = new MainController();
            for (var i = 0; i < 12; i++)
            {
                var res = controller.Create();
                Assert.AreEqual(res.status, "ok");

                for (byte n = tests[i,0]; n > 0; n--)
                {
                    var res2 = controller.Add(new ObservationRequest
                    {
                        sequence = res.response.sequence,
                        observation = new Observation
                        {
                            color = "green",
                            numbers = controller.EmulateBrokenDigits(n, tests[i,1], tests[i,2])
                        }
                    });
                    Assert.IsInstanceOfType(res2, typeof(ObservationResponse));
                }

                var res3 = controller.Add(new ObservationRequest
                {
                    sequence = res.response.sequence,
                    observation = new Observation
                    {
                        color = "red",
                    }
                });

                Assert.IsInstanceOfType(res3, typeof(ObservationResponse));

                Assert.AreEqual(((ObservationResponse)res3).numbers[0], tests[i,0]);


            }
            


        }

        

        
    }
}