﻿using ScribrAPI.Controllers;
using ScribrAPI.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace UnitTestScribrAPI
{
    [TestClass]
    public class TranscriptionsControllerUnitTests
    {
        public static readonly DbContextOptions<scriberContext> options
        = new DbContextOptionsBuilder<scriberContext>()
            .UseInMemoryDatabase(databaseName: "testDatabase")
            .Options;

        public static readonly IList<Transcription> transcriptions = new List<Transcription>
        {
            new Transcription()
            {
                Phrase = "That's like calling"
            },
            new Transcription()
            {
                Phrase = "A peanut butter sandwich"
            }
        };

        [TestInitialize]

        public void SetupDb()
        {
            using (var context = new scriberContext(options))
            {
                context.Transcription.Add(transcriptions[0]);
                context.Transcription.Add(transcriptions[1]);
                context.SaveChanges();
            }
        }

        [TestCleanup]
        public void ClearDb()
        {
            using (var context = new scriberContext(options))
            {
                // clear the db
                context.Transcription.RemoveRange(context.Transcription);
                context.SaveChanges();
            };
        }
        [TestMethod]
        public async Task TestGetSuccessfully()
        {
            using (var context = new scriberContext(options))
            {
                TranscriptionsController transcriptionsController = new TranscriptionsController(context);
                ActionResult<IEnumerable<Transcription>> result = await transcriptionsController.GetTranscription();

                Assert.IsNotNull(result);
            }
        }
        [TestMethod]
        public async Task TestPutTranscriptionNoContentStatus()
        {
            using (var context = new scriberContext(options))
            {
                string phrase = "this is now a different phrase";
                Transcription transcription1 = context.Transcription.Where(
                    x => x.Phrase == transcriptions[0].Phrase).Single();
                transcription1.Phrase = phrase;

                TranscriptionsController transcriptionsController = new TranscriptionsController(context);
                IActionResult result = await transcriptionsController.PutTranscription(transcription1.TranscriptionId, transcription1) as IActionResult;

                Assert.IsNotNull(result);
                Assert.IsInstanceOfType(result, typeof(NoContentResult));
            }
        }
    }
}