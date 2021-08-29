using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using Relojes.Common.Models;
using Relojes.Common.Responses;
using Relojes.Functions.Entities;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Relojes.Functions.Functions
{
    public static class TodoApi
    {
        [FunctionName(nameof(CreateTodo))]
        public static async Task<IActionResult> CreateTodo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Table1")] HttpRequest req,
            [Table("Table1", Connection = "AzureWebJobsStorage")] CloudTable todoTable1,
            ILogger log)
        {
            log.LogInformation("Recibida un nuevo Registro");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Table1 todo1 = JsonConvert.DeserializeObject<Table1>(requestBody);


            if (string.IsNullOrEmpty(todo1?.ID) ||
                todo1?.Fecha == null ||
                string.IsNullOrEmpty(todo1?.Tipo))
            {
                return new BadRequestObjectResult(new Responses
                {
                    IsSucces = false,
                    Message = "Es Requisito Ingresar un ID un Tipo o Fecha"
                });
            }

            Table1Entity todoEntity = new Table1Entity
            {
                ETag = "*",
                Consolidado = false,
                PartitionKey = "Table1",
                RowKey = Guid.NewGuid().ToString(),
                ID = todo1.ID,
                Tipo = todo1.Tipo,
                Fecha = todo1.Fecha
            };

            TableOperation addOperation = TableOperation.Insert(todoEntity);
            await todoTable1.ExecuteAsync(addOperation);

            string message = "Nuevo Registro en la Tabla";
            log.LogInformation(message);

            return new OkObjectResult(new Responses
            {
                IsSucces = true,
                Message = message,
                Result = todoEntity
            });
        }

        [FunctionName(nameof(UpdateTodo))]
        public static async Task<IActionResult> UpdateTodo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "Table1/{Rowkey}")] HttpRequest req,
            [Table("Table1", Connection = "AzureWebJobsStorage")] CloudTable todoTable1,
            String Rowkey,
            ILogger log)
        {
            log.LogInformation($"Actualización en la tabla del {Rowkey}, Recibido");

            String requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Table1 todoRequest = JsonConvert.DeserializeObject<Table1>(requestBody);

            // Validate report i
            TableOperation findOperation = TableOperation.Retrieve<Table1Entity>("Table1", Rowkey);
            TableResult findResult = await todoTable1.ExecuteAsync(findOperation);
            if (findResult.Result == null)
            {
                return new NotFoundObjectResult(new Responses
                {
                    IsSucces = false,
                    Message = "Id no Encontrado"
                });
            }

            // Update information for the todo
            Table1Entity todoEntity = (Table1Entity)findResult.Result;
            todoEntity.Consolidado = todoRequest.Consolidado;
            if (!string.IsNullOrEmpty(todoRequest.ID))

            {
                todoEntity.ID = todoRequest.ID; 
            }

            if (todoRequest.Fecha != null)
            {
                todoEntity.Fecha = todoRequest.Fecha;
            }

            if (!string.IsNullOrEmpty(todoRequest.Tipo))

            {
                todoEntity.Tipo = todoRequest.Tipo;
            }

            TableOperation replaceOperation = TableOperation.Replace(todoEntity);
            await todoTable1.ExecuteAsync(replaceOperation);

            string message = $"Actualización en la tabla del ID:{Rowkey}";
            log.LogInformation(message);

            return new OkObjectResult(new Responses
            {
                IsSucces = true,
                Message = message,
                Result = todoEntity
            });
        }


    }
}


