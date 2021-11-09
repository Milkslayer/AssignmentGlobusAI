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
        
        #endregion
        
        
        private TicketProcessor _ticketProcessor;
        
        [SetUp]
        public void Initialize()
        {
            _ticketProcessor = new TicketProcessor();
        }

        private void InitializeWithData(Dictionary<int, Ticket> tickets)
        {
            _ticketProcessor = new TicketProcessor(tickets);
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
        public void CloseTicket_TicketDoesNotExist_ReturnFalseNull(int ticketToClose, Dictionary<int, Ticket> tickets)
        {
            InitializeWithData(tickets);
            
            var (isSuccess, ticketState) = _ticketProcessor.CloseTicket(ticketToClose);

            Assert.IsFalse(isSuccess);
            Assert.IsNull(ticketState);
        }
        
        [TestCaseSource(nameof(_closeTicketAlreadyClosed))]
        public void CloseTicket_TicketAlreadyClosed_ReturnFalseClosed(int ticketToClose, Dictionary<int, Ticket> tickets)
        {
            InitializeWithData(tickets);
            
            var (isSuccess, ticketState) = _ticketProcessor.CloseTicket(ticketToClose);

            Assert.IsFalse(isSuccess);
            Assert.AreEqual(Ticket.State.Closed, ticketState);
        }
        
        [TestCaseSource(nameof(_closeTicketExists))]
        public void CloseTicket_TicketExists_ReturnTrueClosed(int ticketToClose, Dictionary<int, Ticket> tickets)
        {
            InitializeWithData(tickets);
            
            var (isSuccess, ticketState) = _ticketProcessor.CloseTicket(ticketToClose);

            Assert.IsTrue(isSuccess);
            Assert.AreEqual(Ticket.State.Closed, ticketState);
        }

        [TestCaseSource(nameof(_assignTicketInvalidData))]
        public void AssignTicket_TicketInvalidData_ReturnsFalse(int ticketId, string username,
            Dictionary<int, Ticket> tickets)
        {
            InitializeWithData(tickets);
            
            var actualResult = _ticketProcessor.AssignTicket(ticketId, username);
            
            Assert.IsFalse(actualResult);
        }

        [TestCaseSource(nameof(_assignTicketValidData))]
        public void AssignTicket_ValidDataProvided_ReturnTrue(int ticketId, string username,
            Dictionary<int, Ticket> tickets)
        {
            InitializeWithData(tickets);
            
            var actualResult = _ticketProcessor.AssignTicket(ticketId, username);
            
            Assert.IsTrue(actualResult);
        }
    }
}