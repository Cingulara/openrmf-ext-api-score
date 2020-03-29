// Copyright (c) Cingulara LLC 2019 and Tutela LLC 2019. All rights reserved.
// Licensed under the GNU GENERAL PUBLIC LICENSE Version 3, 29 June 2007 license. See LICENSE file in the project root for full license information.
using System;
using Microsoft.AspNetCore.Mvc;
using openrmf_ext_api_score.Classes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace openrmf_ext_api_score.Controllers
{
    [Route("/")]
    public class ScoreController : Controller
    {
       private readonly ILogger<ScoreController> _logger;

        public ScoreController(ILogger<ScoreController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// POST Called from the OpenRMF UI (or external access) to generate the score of a checklist for the 
        /// category 1, 2, 3 items based on status. This is called from the Template page OR called from any 
        /// checklist listing where the score is currently empty for some reason.
        /// </summary>
        /// <param name="rawChecklist">The actual CKL file text to parse</param>
        /// <returns>
        /// HTTP Status showing it was generated and the score record showing the categories and status numbers.
        /// </returns>
        /// <response code="200">Returns the score generated for the checklist data passed in</response>
        /// <response code="400">If the item did not generate correctly, or if the CKL data was invalid</response>
        [HttpPost]
        [Authorize(Roles = "Administrator,Reader,Editor,Assessor")]
        public IActionResult Score (string rawChecklist){
            try {
                _logger.LogInformation("Calling Score() with a raw Checklist XML data");
                return Ok(ScoringEngine.ScoreChecklistString(rawChecklist));
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Score() Error creating Score for XML string passed in");
                return BadRequest();
            }
        }
    }
}
