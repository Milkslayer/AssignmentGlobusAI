using System;
using System.Collections.Generic;

namespace SupportCli
{
    public class Ticket
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public State CurrentState { get; set; }
        public string AssignedToUser { get; set; }
        public List<string> Comments { get; }

        public Ticket()
        {
            Comments = new List<string>();
        }

        public enum State
        {
            Open,
            InProgress,
            Closed
        }

        public void Show()
        {
            Console.WriteLine($"{nameof(Id)}={Id}");
            Console.WriteLine($"{nameof(Title)}={Title}");
            Console.WriteLine($"{nameof(CurrentState)}={CurrentState}");
            Console.WriteLine($"{nameof(AssignedToUser)}={AssignedToUser}");
            Console.WriteLine($"Comments count={Comments.Count}");
            Console.WriteLine($"{nameof(Comments)}:");
            foreach (var comment in Comments)
            {
                Console.WriteLine($"> {comment}");
            }
        }
    }
}
