using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MultiThreading
{
    class Program
    {

        private static void Main(string[] args)
        {
            // List to hold all the tasks
            var tasks = new List<Task>();

            var tm = new TaksManager();

            tm.QueueTasks(tasks);

            // Wait for all tasks to be finished
            Task.WaitAll(tasks.ToArray());

            Console.WriteLine(Environment.NewLine);
            Console.WriteLine("Done doing all work =) Hit any key to close.");
            Console.ReadLine();
        }

        public class TaksManager
        {
            public void QueueTasks(List<Task> tasks)
            {
                var nodes = GetGraph().OrderBy(x=>x.DependsOn.Any()).ToList();

                // TODO: Replace with graph iterator.
                foreach (var node in nodes)
                {
                    var theNode = node;

                    if (theNode.DependsOn.Any())
                    {
                        theNode.Task = Task.Factory.ContinueWhenAll(theNode.DependsOn.Select(x => x.Task).ToArray(), f => theNode.Action());
                        tasks.Add(theNode.Task);
                    }
                    else
                    {
                        theNode.Task = Task.Factory.StartNew(theNode.Action);
                        tasks.Add(theNode.Task);    
                    }
                }
            }

            public List<Node> GetGraph()
            {
                var node1 = new Node { Id = 1, Action = () => SampleTask.Start(1, 2000)};
                var node2 = new Node { Id = 2, Action = () => SampleTask.Start(2, 2000) };
                var node3 = new Node { Id = 3, DependsOn = new List<Node> { node1, node2 }, Action = () => SampleTask.Start(3, 2000) };
                var node4 = new Node { Id = 4, Action = () => SampleTask.Start(4, 2000) };
                var node5 = new Node { Id = 5, DependsOn = new List<Node> { node3, node4 }, Action = () => SampleTask.Start(5, 2000) };
                var node6 = new Node { Id = 6, DependsOn = new List<Node> { node3, node5 }, Action = () => SampleTask.Start(6, 2000) };

                return new List<Node>
                {
                    node1,
                    node2,
                    node3,
                    node4,
                    node5,
                    node6
                };
            }
        }
    }

    public static class SampleTask
    {
        public static void Start(int workId, int milliseconds)
        {
            Console.WriteLine("Starting work item {0}", workId);
            Thread.Sleep(milliseconds);
            Console.WriteLine("Item {0} is Done", workId);
        }
    }

    public class Node
    {
        public Node()
        {
            DependsOn = new List<Node>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public bool Done { get; set; }
        public Action Action { get; set; }
        public Task Task { get; set; }
        public List<Node> DependsOn { get; set; }
    }
}
