using System;
using System.Collections.Generic;

namespace SupportCli
{
    class Program
    {
        static void Main()
        {
            new TicketProcessor().Start();
        }
    }

    public class TicketProcessor
    {
        private readonly Dictionary<int, Ticket> _tickets = new Dictionary<int, Ticket>();
        private int _counter = 0;

        public void Start()
        {
            Console.WriteLine("\n\nSUPPORT CLI");
            while (true)
            {
                Console.WriteLine("\n\nAvailable commands:");
                Console.WriteLine("create %title% - create a new ticket");
                Console.WriteLine("show %ticket id% - show the ticket");
                Console.WriteLine("comment %ticket id% %comment% - add a comment to the ticket");
                Console.WriteLine("assign %ticket id% %user name% - assign the ticket");
                Console.WriteLine("close %ticket id% - close the ticket");
                Console.WriteLine("list- show all tickets");
                Console.WriteLine("q - exit");
                Console.WriteLine();

                var input = Console.ReadLine();
                if (input.Equals("q")) break;

                if (input.Equals("list"))
                {
                    foreach (var ticket in _tickets)
                    {
                        Console.WriteLine($"{ticket.Value.Id} | {ticket.Value.Title}");
                    }

                    continue;
                }

                if (input.StartsWith("create "))
                {
                    _counter++;
                    _tickets[_counter] = new Ticket
                    {
                        Id = _counter,
                        Title = input.Substring(7, input.Length - 7),
                        CurrentState = Ticket.State.Open,
                        Comments = new List<string>()
                    };
                    Console.WriteLine($"{_counter} has been created ");
                    continue;
                }

                if (input.StartsWith("show "))
                {
                    var id = int.Parse(input.Split(' ')[1]);
                    _tickets[id].Show();
                    continue;
                }

                if (input.StartsWith("comment "))
                {
                    var id = int.Parse(input.Split(' ')[1]);
                    var commentPrefix = 8 + input.Split(' ')[1].Length + 1;
                    var comment = input.Substring(commentPrefix, input.Length - commentPrefix);
                    var ticket = _tickets[id];
                    ticket.Comments.Add(comment);
                    ticket.CommentsCount++;
                    Console.WriteLine($"new comment has been added into {id}");
                    continue;
                }

                if (input.StartsWith("assign "))
                {
                    var id = int.Parse(input.Split(' ')[1]);
                    var usernamePrefix = 7 + input.Split(' ')[1].Length + 1;
                    var username = input.Substring(usernamePrefix, input.Length - usernamePrefix);
                    var ticket = _tickets[id];
                    ticket.Comments.Add("assigned " + username + " " + DateTime.UtcNow);
                    ticket.CommentsCount++;
                    ticket.AssignedToUser = username;
                    ticket.CurrentState = Ticket.State.InProgress;
                    Console.WriteLine($"{id} has been assigned to {username}");
                    continue;
                }

                if (input.StartsWith("close "))
                {
                    var id = int.Parse(input.Split(' ')[1]);
                    var ticket = _tickets[id];
                    ticket.CurrentState = Ticket.State.Closed;
                    ticket.Comments.Add("closed " + DateTime.UtcNow);
                    ticket.CommentsCount++;
                    Console.WriteLine($"{id} has been closed ");
                }
            }
        }
    }
}
