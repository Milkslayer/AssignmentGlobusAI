using System.Collections.Generic;
using NUnit.Framework;
using SupportCli;

namespace SupportCliTests
{
    public class Tests
    {
        #region TestData

        private static Dictionary<int, Ticket> _prepareTicketDict() => new()
        {
            {3, new Ticket(3, "Testing")},
            {6, new Ticket(6, "Close")},
            {9, new Ticket(9, "Ticket")}
        };
        
        private static object[] _createTicketInvalidData =
        {
            new [] {""},
            new [] {string.Empty},
            new string[] {null}
        };
        
        private static object[] _createTicketValidData =
        {
            new [] {new List<string> {"Testing", "titles", "for", "tickets"}},
        };

        private static object[] _closeTicketDoesNotExist =
        {
            new object[] {1, _prepareTicketDict()}
        };

        private static object[] _closeTicketAlreadyClosed =
        {
            new object[] {1, new Dictionary<int, Ticket>
            {
                {1, new Ticket(1, "Test")
                    {
                        CurrentState = Ticket.State.Closed
                    }
                }
            }}
        };

        private static object[] _closeTicketExists =
        {
            new object[] {3, _prepareTicketDict()}
        };

        private static object[] _assignTicketInvalidData =
        {
            new object[] {1, "username", _prepareTicketDict()},
            new object[] {3, "", _prepareTicketDict()},
            new object[] {3, null, _prepareTicketDict()},
            new object[] {3, string.Empty, _prepareTicketDict()},
            new object[] {3, " ",_prepareTicketDict()}
        };
        
        private static object[] _assignTicketValidData =
        {
            new object[] {3, "username", _prepareTicketDict()}
        };


        private static object[] _runCreateTask =
        {
            new object[]
            {
                3, new []
                {
                    "create TicketName",
                    "create 123",
                    "create TicketName 123"
                }
            },
            new object[]
            {
                0, new []
                {
                    "create",
                    "create  ",
                    "create                 132"
                }
            }
        };

        private static object[] _runAddComment =
        {
            new object[]
            {
                6, 3, new[]
                {
                    "comment 6 Comment",
                    "comment 6 123",
                    "comment 6 Comment 123",
                    "comment 5 Comment 123"
                },
                _prepareTicketDict()
            },
            new object[]
            {
                6, 0, new[]
                {
                    "comment Comment",
                    "comment 3123",
                    "comment ",
                    "comment 6     ",
                    "comment 6",
                    "comment 6 ",
                },
                _prepareTicketDict()
            }
        };

        private static object[] _runAssignTicket =
        {
            new object[]
            {
                3, "GlobusAi", new[]
                {
                    "assign 3 GlobusAi",
                },
                _prepareTicketDict()
            },
            new object[]
            {
                3, "123", new[]
                {
                    "assign 3 123",
                },
                _prepareTicketDict()
            },
            new object[]
            {
                3, "GlobusAi 123", new[]
                {
                    "assign 3 GlobusAi 123",
                    "assign 5 GlobusAi 123"
                },
                _prepareTicketDict()
            },
            new object[]
            {
                3, null, new[]
                {
                    "assign GlobusAi",
                    "assign 12312",
                    "assign ",
                    "assign 3 ",
                    "assign 3       ",
                    "assign 3",
                },
                _prepareTicketDict()
            }
        };
        #endregion
        
        
        private TicketProcessor _ticketProcessor;
        
        [SetUp]
        public void Initialize()
        {
            _ticketProcessor = new TicketProcessor();
        }

        private void InitializeWithData(Dictionary<int, Ticket> testData)
        {
            _ticketProcessor = new TicketProcessor(testData);
        }

        [TestCaseSource(nameof(_createTicketInvalidData))]
        public void CreateTicket_NoTicketNameProvided_ReturnsZero(string ticketTitle)
        {
            int expectedId = 0;
            
            var actualId = _ticketProcessor.CreateTicket(ticketTitle);
            Assert.AreEqual(expectedId, actualId);
        }
        
        [TestCaseSource(nameof(_createTicketValidData))]
        public void CreateTicket_TicketNameProvided_ReturnsTicketId(List<string> ticketTitles)
        {
            var expectedIdCounter = 1;
            foreach (var title in ticketTitles)
            { 
                int actualId = _ticketProcessor.CreateTicket(title);
                Assert.AreEqual(expectedIdCounter, actualId);

                expectedIdCounter++;
            }
        }

        [TestCaseSource(nameof(_closeTicketDoesNotExist))]
        public void CloseTicket_TicketDoesNotExist_ReturnFalseNull(int ticketToClose, Dictionary<int, Ticket> testData)
        {
            InitializeWithData(testData);
            
            var (isSuccess, ticketState) = _ticketProcessor.CloseTicket(ticketToClose);

            Assert.IsFalse(isSuccess);
            Assert.IsNull(ticketState);
        }
        
        [TestCaseSource(nameof(_closeTicketAlreadyClosed))]
        public void CloseTicket_TicketAlreadyClosed_ReturnFalseClosed(int ticketToClose, Dictionary<int, Ticket> testData)
        {
            InitializeWithData(testData);
            
            var (isSuccess, ticketState) = _ticketProcessor.CloseTicket(ticketToClose);

            Assert.IsFalse(isSuccess);
            Assert.AreEqual(Ticket.State.Closed, ticketState);
        }
        
        [TestCaseSource(nameof(_closeTicketExists))]
        public void CloseTicket_TicketExists_ReturnTrueClosed(int ticketToClose, Dictionary<int, Ticket> testData)
        {
            InitializeWithData(testData);
            
            var (isSuccess, ticketState) = _ticketProcessor.CloseTicket(ticketToClose);

            Assert.IsTrue(isSuccess);
            Assert.AreEqual(Ticket.State.Closed, ticketState);
        }

        [TestCaseSource(nameof(_assignTicketInvalidData))]
        public void AssignTicket_TicketInvalidData_ReturnsFalse(int ticketId, string username,
            Dictionary<int, Ticket> testData)
        {
            InitializeWithData(testData);
            
            var actualResult = _ticketProcessor.AssignTicket(ticketId, username);
            
            Assert.IsFalse(actualResult);
        }

        [TestCaseSource(nameof(_assignTicketValidData))]
        public void AssignTicket_ValidDataProvided_ReturnTrue(int ticketId, string username,
            Dictionary<int, Ticket> testData)
        {
            InitializeWithData(testData);
            
            var actualResult = _ticketProcessor.AssignTicket(ticketId, username);
            
            Assert.IsTrue(actualResult);
        }

        [TestCaseSource(nameof(_runCreateTask))]
        public void RunCreateTask(int expectedTicketCount,
            IEnumerable<string> commands)
        {
            foreach (var command in commands)
            {
                _ticketProcessor.RunCreateTicket(command);
            }
            Assert.AreEqual(expectedTicketCount, _ticketProcessor.GetTicketCount());
        }
        
        [TestCaseSource(nameof(_runAddComment))]
        public void RunAddComment(int ticketId,
            int expectedCommentCount,
            IEnumerable<string> commands,
            Dictionary<int, Ticket> testData)
        {
            InitializeWithData(testData);

            foreach (var command in commands)
            {
                _ticketProcessor.RunAddComment(command);
            }

            var ticket = _ticketProcessor.FindTicketById(ticketId);
            Assert.AreEqual(expectedCommentCount, ticket.Comments.Count);
        }
        
        [TestCaseSource(nameof(_runAssignTicket))]
        public void RunAssignTicket(int ticketId,
            string expectedAssignee,
            IEnumerable<string> commands,
            Dictionary<int, Ticket> testData)
        {
            InitializeWithData(testData);
            foreach (var command in commands)
            {
                _ticketProcessor.RunAssignTicket(command);
            }
            var ticket = _ticketProcessor.FindTicketById(ticketId);

            Assert.AreEqual(expectedAssignee, ticket.AssignedToUser);
        }
    }
}