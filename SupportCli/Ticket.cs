using System;
using System.Collections.Generic;

namespace SupportCli
{
    public class Ticket
    {
        public enum State
        {
            Open,
            InProgress,
            Closed
        }
        
        public int Id { get; set; }
        public string Title { get; set; }
        public State CurrentState { get; set; }
        public string AssignedToUser { get; set; }
        public List<string> Comments { get; }
        
        public Ticket(int id, string title)
        {
            Comments = new List<string>();
            Id = id;
            Title = title;
            CurrentState = State.Open;
        }

        public void Show()
        {
            Console.WriteLine($"{nameof(Id)}={Id}");
            Console.WriteLine($"{nameof(Title)}={Title}");
            Console.WriteLine($"{nameof(CurrentState)}={CurrentState}");
            Console.WriteLine($"{nameof(AssignedToUser)}={AssignedToUser}");
            Console.WriteLine($"CommentsCount={Comments.Count}");
            Console.WriteLine($"{nameof(Comments)}:");
            foreach (var comment in Comments)
            {
                Console.WriteLine($"# {comment}");
            }
        }
    }
}
