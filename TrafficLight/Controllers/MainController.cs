using System.Collections;
using System.Web.Http;
using TestTrafficLight.Models;

namespace TestTrafficLight.Controllers
{
    /// <summary>
    /// WEB API
    /// </summary>
    public class MainController : ApiController
    {
        
        /// <summary>
        ///Перед началом отправки данных каждый наблюдатель делает запрос в сервис на 
        ///предоставление уникального кода его текущей последовательности. Последующие 
        ///данные о наблюдениях сопровождаются этим кодом. 
        /// </summary>
        /// <returns>
        /// Код последовательности
        /// </returns>
        public CreateResponse Create()
        {
            return new CreateResponse(TaskSolver.Instance.Create());
        }

        
        /// <summary>
        /// Функция добавления очередного наблюдения
        /// </summary>
        /// <param name="request">Данные наблюдения</param>
        /// <returns>Результат рассчета</returns>
        public BaseResponse Add(ObservationRequest request)
        {
            return TaskSolver.Instance.Add(request);
        }


        /// <summary>
        /// Очистка БД
        /// </summary>
        /// <returns></returns>
        public ClearResponse Clear()
        {
            
            
            return TaskSolver.Instance.Clear();


        }

        
        /// <summary>
        /// Вспомогательная функция для просмотра содержимого таблиц
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public IList Show(string tableName)
        {
            return TaskSolver.Instance.Show(tableName);

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
            return TaskSolver.Instance.EmulateBrokenDigits(value, mask1, mask2);
        }
    }

   
}
