//BEGIN LICENSE BLOCK 
//Interneuron Autonomic

//Copyright(C) 2022  Interneuron CIC

//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.

//See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with this program.If not, see<http://www.gnu.org/licenses/>.
//END LICENSE BLOCK 


using System;
using System.Collections.Generic;
using Interneuron.Autonomic.Models;
using Interneuron.Autonomic.Services;
using Interneuron.Autonomic.Helpers;
using InterneuronAutonomic.API;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Interneuron.Autonomic.Controllers
{
    [Authorize]
    [Route("/Autonomic/EWS/")]
    [ApiController]
    public class EWSController : ControllerBase
    {
        DynamicAPIClient _dynamicAPIClient;
        public EWSController(DynamicAPIClient dynamicAPIClient)
        {
            _dynamicAPIClient = dynamicAPIClient;
        }

        [HttpGet]
        [Route("[action]/{observationEventId}")]
        public ActionResult<string> CalculateEWS(string observationEventId)
        {

            EWSResponse ewsResponse = new EWSResponse();

            // Get Observation Event via autonomic_observationevent baseview and the DynamicAPI client.
            string obsEventJson = _dynamicAPIClient.GetBaseViewListObjectByAttribute("autonomic_observationevent", "observationevent_id", observationEventId);

            // Deserialize json to ObservationEvent model
            ObservationEvent obsEvent = JsonConvert.DeserializeObject<ObservationEvent>(obsEventJson);

            // Invoke the EWSCalculator service which returns an EWSResponse for the relevant scale type.
            if (obsEvent.scaletype == "NEWS2-Scale1" || obsEvent.scaletype == "NEWS2-Scale2")
            {
                //NEWS2 scale
                ewsResponse = EWSCalculator.CalculateNEWS2Score(obsEvent);
            }
            else if (obsEvent.scaletype == "PEWS-0To11Mo" || obsEvent.scaletype == "PEWS-1To4Yrs" || obsEvent.scaletype == "PEWS-5To12Yrs" || obsEvent.scaletype == "PEWS-13To18Yrs")
            {
                //PEWS scale
                ewsResponse = EWSCalculator.CalculatePEWSScore(obsEvent);
            }
            else
            {
                //Not a valid scale type
                ewsResponse.status = "ERROR";
                ewsResponse.error = "Could not find a valid scale type for this Observation Event";
                ewsResponse.score = null;
            }
            

            var useridClaim = User.FindFirst("IPUId");
            var userId = "unknown";

            if (useridClaim != null)
            {
                userId = useridClaim.Value;
            }

            if (ewsResponse.status == "SUCCESS")
            {
                //Create Score to persist.
                Score ewsScore = new Score();
                ewsScore.score_id = ewsResponse.score_id;
                ewsScore.person_id = obsEvent.person_id;
                ewsScore.encounter_id = ""; //TODO
                ewsScore.observationevent_id = obsEvent.observationevent_id;
                ewsScore.score = ewsResponse.score;
                ewsScore.scoretype = ewsResponse.score_type;
                ewsScore.guidance = ewsResponse.guidance;
                ewsScore.calculatedby = userId;
                ewsScore.calculateddatetime = ewsResponse.score_datetime;
                ewsScore.calculatedsystem = "Interneuron.Autonomic";

                //Create Score Parameter List
                List<ScoreParameter> ewsScoreParamList = new List<ScoreParameter>();

                ewsScoreParamList = EWSHelper.CopyEWSScoreParameterList(ewsResponse.parameters, ewsResponse.score_id, ewsResponse.observationevent_id);

                // post the score to the dynamic API
                string scoreResponse = _dynamicAPIClient.PostObject("core", "score", ewsScore);

                // delete existing scoreparameters
                string scoreParamDeleteResponse = _dynamicAPIClient.DeleteObjectByAttribute("core", "scoreparameter", "score_id", ewsResponse.score_id);

                // persist the new score parameters
                string scoreParamResponse = _dynamicAPIClient.PostObjectArray("core", "scoreparameter", ewsScoreParamList);
            }

            return JsonConvert.SerializeObject(ewsResponse);
        }

    }
}
