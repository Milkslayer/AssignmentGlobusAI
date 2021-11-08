using System;
using System.Collections.Generic;

namespace SupportCli
{
    public class TicketProcessor
    {
        private readonly Dictionary<int, Ticket> _tickets = new();
        private int _counter;
        
        private readonly Dictionary<string, string> _commands = new()
        {
            {"assign", "assign "},
            {"comment", "comment "},
            {"close", "close "},
            {"create", "create "},
            {"help", "help"},
            {"list", "list"},
            {"show", "show "},
            {"quit", "quit"},
        };

        public void Start()
        {
            Console.WriteLine("\n\nSUPPORT CLI *");
            Console.WriteLine("*************");
            Console.WriteLine("Type 'help' for available commands");
            while (true)
            {
                
                var input = Console.ReadLine();

                if (input.Equals(_commands["quit"])) break;
                
                if (input.Equals(_commands["help"]))
                {
                    PrintCommandList();
                    continue;
                }

                if (input.Equals(_commands["list"]))
                {
                    ListTickets();
                    continue;
                }

                if (input.StartsWith(_commands["create"]))
                {
                    string ticketName = input.Substring(_commands["create"].Length,
                        input.Length - _commands["create"].Length);
                    CreateTicket(ticketName);
                    Console.WriteLine($"{_counter} has been created ");
                    continue;
                }

                if (input.StartsWith(_commands["show"]))
                {
                    var id = int.Parse(input.Split(' ')[1]);
                    ShowTicket(id);
                    continue;
                }

                if (input.StartsWith(_commands["comment"]))
                {
                    var id = int.Parse(input.Split(' ')[1]);
                    var commentPrefix = _commands["comment"].Length + input.Split(' ')[1].Length + 1;
                    var comment = input.Substring(commentPrefix, input.Length - commentPrefix);
                    var ticket = _tickets[id];
                    ticket.Comments.Add(comment);
                    
                    
                    Console.WriteLine($"new comment has been added into {id}");
                    continue;
                }

                if (input.StartsWith(_commands["assign"]))
                {
                    var id = int.Parse(input.Split(' ')[1]);
                    var usernamePrefix = 7 + input.Split(' ')[1].Length + 1;
                    var username = input.Substring(usernamePrefix, input.Length - usernamePrefix);
                    var ticket = _tickets[id];
                    ticket.Comments.Add("assigned " + username + " " + DateTime.UtcNow);
                    ticket.AssignedToUser = username;
                    ticket.CurrentState = Ticket.State.InProgress;
                    Console.WriteLine($"{id} has been assigned to {username}");
                    continue;
                }

                if (input.StartsWith(_commands["close"]))
                {
                    var id = int.Parse(input.Split(' ')[1]);
                    var ticket = _tickets[id];
                    ticket.CurrentState = Ticket.State.Closed;
                    ticket.Comments.Add("closed " + DateTime.UtcNow);
                    Console.WriteLine($"{id} has been closed ");
                }
            }
        }

        private void ShowTicket(int id)
        {
            _tickets[id].Show();
        }

        public int CreateTicket(string ticketTitle)
        {
            if (string.IsNullOrWhiteSpace(ticketTitle)) return 0;
            
            _counter++;
            _tickets[_counter] = new Ticket
            {
                Id = _counter,
                Title = ticketTitle,
                CurrentState = Ticket.State.Open,
            };
            return _counter;
        }

        public void ListTickets()
        {
            foreach (var ticket in _tickets)
            {
                Console.WriteLine($"{ticket.Value.Id} | {ticket.Value.Title}");
            }
        }

        public void PrintCommandList()
        {
            Console.WriteLine("\nAvailable commands:");
            Console.WriteLine("create %title% - create a new ticket");
            Console.WriteLine("show %ticket id% - show the ticket");
            Console.WriteLine("comment %ticket id% %comment% - add a comment to the ticket");
            Console.WriteLine("assign %ticket id% %user name% - assign the ticket");
            Console.WriteLine("close %ticket id% - close the ticket");
            Console.WriteLine("list- show all tickets");
            Console.WriteLine("q - exit");
            Console.WriteLine();
        }
    }
}