using System;
using System.Collections.Generic;

namespace SupportCli
{
    public class TicketProcessor
    {
        private readonly Dictionary<int, Ticket> _tickets;
        private int _counter;

        public TicketProcessor()
        {
            _tickets = new Dictionary<int, Ticket>();
        }

        public TicketProcessor(Dictionary<int, Ticket> tickets)
        {
            _tickets = tickets;
        }

        private readonly Dictionary<string, string> _commands = new()
        {
            {"assign", "assign"},
            {"comment", "comment "},
            {"close", "close "},
            {"create", "create "},
            {"help", "help"},
            {"list", "list"},
            {"show", "show "},
            {"quit", "quit"},
        };
        
        public int GetTicketCount() => _counter;

        public Ticket FindTicketById(int ticketId)
        {
            if (_tickets.ContainsKey(ticketId))
                return _tickets[ticketId];
            return null;
        }
        
        public void Start()
        {
            Console.WriteLine("SUPPORT CLI *");
            Console.WriteLine("*************");
            while (true)
            {
                Console.Write("> ");
                var input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                    continue;
                
                if (input.Equals(_commands["quit"])) 
                    break;
                
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
                    RunCreateTicket(input);
                    continue;
                }

                if (input.StartsWith(_commands["show"]))
                {
                    if (int.TryParse(input.Split(' ')[1], out int id)) 
                        ShowTicket(id);
                    else
                        Console.WriteLine("Invalid ticket Id entered");
                    continue;
                }

                if (input.StartsWith(_commands["comment"]))
                {
                    RunAddComment(input);
                    continue;
                }

                if (input.StartsWith(_commands["assign"]))
                {
                    RunAssignTicket(input);
                    continue;
                }

                if (input.StartsWith(_commands["close"]))
                {
                    RunCloseTask(input);
                    continue;
                }

                Console.WriteLine("\n\nType 'help' for available commands");
            }
        }

        private void RunCloseTask(string rawInput)
        {
            if (int.TryParse(rawInput.Split(' ')[1], out int id))
            {
                var (isSuccess, ticketState) = CloseTicket(id);

                switch (isSuccess)
                {
                    case false when ticketState == null:
                        Console.WriteLine($"Ticket {id} not found");
                        break;
                    case false when ticketState == Ticket.State.Closed:
                        Console.WriteLine($"Ticket {id} is already closed");
                        break;
                    default:
                        Console.WriteLine($"Ticket {id} has been closed successfully");
                        break;
                }
            }
            else
                Console.WriteLine("Invalid ticket Id entered");
        }
        
        public void RunAssignTicket(string rawInput)
        {
            if (int.TryParse(rawInput.Split(' ')[1], out int id))
            {
                try
                {
                    var usernamePrefix = _commands["assign"].Length + 1 + rawInput.Split(' ')[1].Length + 1;
                    var username = rawInput.Substring(usernamePrefix, rawInput.Length - usernamePrefix);

                    if (string.IsNullOrWhiteSpace(username))
                    {
                        Console.WriteLine("Username cannot be empty");
                        return;
                    }

                    Console.WriteLine(AssignTicket(id, username)
                        ? $"Ticket {id} has been assigned to {username}"
                        : "Ticket not found");
                }
                catch (ArgumentOutOfRangeException)
                {
                    Console.WriteLine("Username not provided");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
            else
                Console.WriteLine("Invalid ticket Id entered");
        }

        public void RunCreateTicket(string rawInput)
        {
            string ticketName = rawInput.Substring(_commands["create"].Length,
                rawInput.Length - _commands["create"].Length);
            int ticketId = CreateTicket(ticketName);
            Console.WriteLine(ticketId != 0 ? $"Ticket {ticketId} has been created" : "Invalid ticket name entered");
        }

        public void RunAddComment(string input)
        {
            if (int.TryParse(input.Split(' ')[1], out int id))
            {
                if (!_tickets.ContainsKey(id))
                {
                    Console.WriteLine($"Ticket {id} not found");
                    return;
                }
                try
                {
                    var commentPrefix = _commands["comment"].Length + input.Split(' ')[1].Length + 1;
                    var comment = input.Substring(commentPrefix, input.Length - commentPrefix);

                    if (string.IsNullOrWhiteSpace(comment))
                    {
                        Console.WriteLine("Comment cannot be empty");
                        return;
                    }
                        
                    var ticket = _tickets[id];
                    ticket.Comments.Add(comment);
                    
                    Console.WriteLine($"New comment has been added into {id}");
                }
                catch (ArgumentOutOfRangeException)
                {
                    Console.WriteLine("Comment not provided");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }
        
        public bool AssignTicket(int id, string username)
        {
            if (!_tickets.ContainsKey(id) || string.IsNullOrWhiteSpace(username))
                return false;
            
            var ticket = _tickets[id];
            ticket.Comments.Add($"assigned {username} {DateTime.UtcNow}");
            ticket.AssignedToUser = username;
            ticket.CurrentState = Ticket.State.InProgress;
            return true;
        }
        
        public (bool, Ticket.State?) CloseTicket(int id)
        {
            if (!_tickets.ContainsKey(id))
                return (false, null);
            
            var ticket = _tickets[id];

            if (ticket.CurrentState == Ticket.State.Closed)
                return (false, ticket.CurrentState);
            
            ticket.CurrentState = Ticket.State.Closed;
            ticket.Comments.Add($"closed {DateTime.UtcNow}");
            return (true, ticket.CurrentState);
        }

        public int CreateTicket(string ticketTitle)
        {
            if (string.IsNullOrWhiteSpace(ticketTitle)) return 0;
            
            _counter++;
            _tickets[_counter] = new Ticket(_counter, ticketTitle);
            return _counter;
        }
        
        private void ShowTicket(int id)
        {
            if (_tickets.ContainsKey(id))
                _tickets[id].Show();
            else 
                Console.WriteLine($"Ticket with {id} does not exist");
        }

        private void ListTickets()
        {
            Console.WriteLine($"Id | Title");
            foreach (var ticket in _tickets)
            {
                Console.WriteLine($"{ticket.Value.Id} | {ticket.Value.Title}");
            }
        }

        private void PrintCommandList()
        {
            Console.WriteLine();
            Console.WriteLine("create %title% - create a new ticket");
            Console.WriteLine("show %ticket id% - show the ticket");
            Console.WriteLine("comment %ticket id% %comment% - add a comment to the ticket");
            Console.WriteLine("assign %ticket id% %user name% - assign the ticket");
            Console.WriteLine("close %ticket id% - close the ticket");
            Console.WriteLine("list- show all tickets");
            Console.WriteLine("quit- exit");
            Console.WriteLine();
        }
    }
}