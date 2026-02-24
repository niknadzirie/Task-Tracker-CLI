using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml;
using Microsoft.VisualBasic;

namespace Task_Tracker_CLI;
internal class Program
{
    private static void Main(string[] args)
    {
        //JSON File
        List<TaskProperties> taskClis = [];
        string filePath = "data.json";

        if (File.Exists(filePath))
        {
            var jsonData = File.ReadAllText(filePath);
            taskClis = JsonSerializer.Deserialize<List<TaskProperties>>(jsonData, new JsonSerializerOptions { WriteIndented = true, Converters = { new JsonStringEnumConverter() } });
        }
        else
        {
            File.WriteAllText(filePath, "[]");
        }

        //Write Data
        void WriteData(List<TaskProperties> data)
        {
            string jsonString = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true, Converters = { new JsonStringEnumConverter() } });
            File.WriteAllText(filePath, jsonString);
        }

        //Add task
        void AddNewData(string description, TaskStatusEnum status = TaskStatusEnum.Todo)
        {
            var newItem = new TaskProperties { Id = Guid.NewGuid(), Description = description, Status = status, CreatedAt = DateTimeOffset.Now, UpdatedAt = DateTimeOffset.Now };
            taskClis.Add(newItem);
            WriteData(taskClis);
            Console.WriteLine($"Task Added Successfully (ID: {newItem.Id})");
        }

        //Update task
        void UpdateData(Guid targetedId, string description, TaskStatusEnum status = TaskStatusEnum.Todo)
        {
            var itemToUpdate = taskClis.FirstOrDefault(c => c.Id == targetedId);
            if (itemToUpdate != null)
            {
                itemToUpdate.Description = description;
                itemToUpdate.Status = status;
                itemToUpdate.UpdatedAt = DateTimeOffset.Now;
            }
            else
            {
                Console.WriteLine("Data not found!");
            }

            WriteData(taskClis);
        }

        //Update Status
        void UpdateStatus(Guid targetedId, TaskStatusEnum status)
        {
            var itemToUpdate = taskClis.FirstOrDefault(c => c.Id == targetedId);
            if (itemToUpdate != null)
            {
                itemToUpdate.Status = status;
                itemToUpdate.UpdatedAt = DateTimeOffset.Now;
            }
            else
            {
                Console.WriteLine($"Data Not Found!");
            }
            WriteData(taskClis);
        }

        //Delete Task
        void DeleteData(Guid targetId)
        {
            var itemToRemove = taskClis.FirstOrDefault(c => c.Id == targetId);
            if (itemToRemove != null)
            {
                taskClis.Remove(itemToRemove);
                Console.WriteLine($"Data with ID: {itemToRemove.Id} removed");
            }
            else
            {
                Console.WriteLine("Data not found!");
            }
            WriteData(taskClis);
        }

        //List all task
        void ListAllTask()
        {
            foreach (var task in taskClis)
            {
                Console.WriteLine($"\nID: {task.Id}\nTask: {task.Description}\nStatus: {task.Status}\nCreated At: {task.CreatedAt}\nUpdated At: {task.UpdatedAt}\n\n");
            }
        }

        void ListTaskBasedOnStatus(string[] args)
        {
            string statusArg = args[1].Replace("-", "");
            if (Enum.TryParse<TaskStatusEnum>(statusArg, true, out var status))
            {
                var filteredTasks = taskClis.Where(c => c.Status == status).ToList();
                if (filteredTasks.Any())
                {
                    foreach (var task in filteredTasks)
                    {
                        Console.WriteLine($"\nID: {task.Id}\nTask: {task.Description}\nStatus: {task.Status}\nCreated At: {task.CreatedAt}\nUpdated At: {task.UpdatedAt}\n\n");
                    }
                }
                else
                {
                    Console.WriteLine($"No task found with status: {status}");
                }
            }
            else
            {
                Console.WriteLine("Invalid status. Use: Todo, Inprogress, Done");
            }
        }

        switch (args[0].ToLower())
        {
            case "add":
                AddNewData(args[1]);
                break;

            case "list":
                if (args.Length > 1)
                {
                    ListTaskBasedOnStatus(args);
                    return;
                }
                else
                {
                    ListAllTask();
                }
                break;

            case "update":
                UpdateData(Guid.Parse(args[1]), args[2]);
                break;

            case "delete":
                DeleteData(Guid.Parse(args[1]));
                break;

            case "mark-in-progress":
                UpdateStatus(Guid.Parse(args[1]), TaskStatusEnum.InProgress);
                break;

            case "mark-done":
                UpdateStatus(Guid.Parse(args[1]), TaskStatusEnum.Done);
                break;

            case "mark-todo":
                UpdateStatus(Guid.Parse(args[1]), TaskStatusEnum.Todo);
                break;

            default:
                break;
        }
    }
}
