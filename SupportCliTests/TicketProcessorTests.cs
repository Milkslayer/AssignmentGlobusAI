using System.Collections.Generic;
using NUnit.Framework;
using SupportCli;

namespace SupportCliTests
{
    public class Tests
    {
        #region TestData

        private static object[] CreateTicket_InvalidData =
        {
            new [] {""},
            new [] {string.Empty},
            new string[] {null}
        };
        
        private static object[] CreateTicket_ValidData =
        {
            new [] {new List<string> {"Tickets"}},
            new [] {new List<string> {"Testing", "tickets"}},
            new [] {new List<string> {"Testing", "titles", "for", "tickets"}},
        };

        #endregion
        
        
        private TicketProcessor _ticketProcessor;
        
        [SetUp]
        public void Setup()
        {
            _ticketProcessor = new TicketProcessor();
        }

        [TestCaseSource(nameof(CreateTicket_InvalidData))]
        public void CreateTicket_NoTicketNameProvided_ReturnsZero(string ticketTitle)
        {
            int expectedId = 0;
            
            var actualId = _ticketProcessor.CreateTicket(ticketTitle);
            Assert.AreEqual(expectedId, actualId);
        }
        
        [TestCaseSource(nameof(CreateTicket_ValidData))]
        public void CreateTicket_TicketNameProvided_ReturnsTicketId(List<string> ticketTitles)
        {
            int expectedIdCounter = 1;
            foreach (var title in ticketTitles)
            { 
                int actualId = _ticketProcessor.CreateTicket(title);
                Assert.AreEqual(expectedIdCounter, actualId);

                expectedIdCounter++;
            }
        }
    }
}