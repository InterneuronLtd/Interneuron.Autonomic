//Interneuron Synapse

//Copyright(C) 2019  Interneuron CIC

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

using System;
using Interneuron.Autonomic.Models;
using Interneuron.Autonomic.Helpers;

namespace Interneuron.Autonomic.Services
{

    public class EWSCalculator
    {
        // CalculateNEWS2Score
        // Requires Resp, Spo2, SBP, Pulse, COnscious Level, Temp and Scale Type to calculate aggregate score.
        // Returns EWSResponse object with Error/Success status flag and ErrorMessage/Guidance.
        public static EWSResponse CalculateNEWS2Score(ObservationEvent observationEvent)
        {
            EWSResponse ewsResponse = new EWSResponse();

            bool redScoreIndicator = false; //will be set to true if an individual score is = 3.

            ewsResponse.observationevent_id = observationEvent.observationevent_id;

            if (String.IsNullOrEmpty(observationEvent.score_id))
            {
                ewsResponse.score_id = Guid.NewGuid().ToString();
            }
            else
            {
                ewsResponse.score_id = observationEvent.score_id;
            }

            ewsResponse.score = 0;
            ewsResponse.score_type = "NEWS2"; 
            ewsResponse.score_datetime = DateTime.Now;

            // check we have a complete set of observations
            if (String.IsNullOrEmpty(observationEvent.resp.ToString()) || String.IsNullOrEmpty(observationEvent.spo2.ToString()) ||
                String.IsNullOrEmpty(observationEvent.bps.ToString()) || String.IsNullOrEmpty(observationEvent.acvpu) ||
                String.IsNullOrEmpty(observationEvent.pulse.ToString()) || String.IsNullOrEmpty(observationEvent.scaletype) ||
                String.IsNullOrEmpty(observationEvent.temp.ToString()))
            {
                ewsResponse.status = "ERROR";
                ewsResponse.error = "Observation event does not contain a full set of observations, unable to calculate score.";
                ewsResponse.score = null;
                return ewsResponse;
            }


            // respiratory units
            if (ValidationHelper.IsValidWholeNumber((Double)observationEvent.resp))
            {
                if (observationEvent.resp <= 8)
                {
                    ewsResponse.score += 3;
                    redScoreIndicator = true;
                }
                else if (observationEvent.resp >= 9 && observationEvent.resp <= 11)
                {
                    ewsResponse.score += 1;
                }
                else if (observationEvent.resp >= 21 && observationEvent.resp <= 24)
                {
                    ewsResponse.score += 2;
                }
                else if (observationEvent.resp >= 25)
                {
                    ewsResponse.score += 3;
                    redScoreIndicator = true;
                }
            }
            else
            {
                // Raise error - null resp rate provided
                ewsResponse.status = "ERROR";
                ewsResponse.error = "No valid respiratory rate value has been recorded in this observation event id.";
                ewsResponse.score = null;
                return ewsResponse;
            }

            //Sp02 - SCALE 1
            //TODO: Confirm value for SCALE1
            if(observationEvent.scaletype == "NEWS2-Scale1" && ValidationHelper.IsValidWholeNumber((Double)observationEvent.spo2))
            {
                if (observationEvent.spo2 <= 91)
                {
                    ewsResponse.score += 3;
                    redScoreIndicator = true;
                }
                else if (observationEvent.spo2 >= 92 && observationEvent.spo2 <= 93)
                {
                    ewsResponse.score += 2;
                }
                else if (observationEvent.spo2 >= 94 && observationEvent.spo2 <= 95)
                {
                    ewsResponse.score += 1;
                }
                
            }
            else if (observationEvent.scaletype == "NEWS2-Scale2" && ValidationHelper.IsValidWholeNumber((Double)observationEvent.spo2))
            {
                if (observationEvent.spo2 <= 83)
                {
                    ewsResponse.score += 3;
                    redScoreIndicator = true;
                }
                else if (observationEvent.spo2 >= 84 && observationEvent.spo2 <= 85)
                {
                    ewsResponse.score += 2;
                }
                else if (observationEvent.spo2 >= 86 && observationEvent.spo2 <= 87)
                {
                    ewsResponse.score += 1;
                }
                else if (observationEvent.spo2 >= 93 && observationEvent.spo2 <= 94 && observationEvent.onoxygen == true)
                {
                    ewsResponse.score += 1;
                }
                else if (observationEvent.spo2 >= 95 && observationEvent.spo2 <= 96 && observationEvent.onoxygen == true)
                {
                    ewsResponse.score += 2;
                }
                else if (observationEvent.spo2 >= 97 && observationEvent.onoxygen == true)
                {
                    ewsResponse.score += 3;
                    redScoreIndicator = true;
                }

            }
            else
            {
                // Raise error, null value
                ewsResponse.status = "ERROR";
                ewsResponse.error = "No valid Spo2 value for the provided scale has been recorded in this observation event id.";
                ewsResponse.score = null;
                return ewsResponse;
            }

            //On Oxygen
            if (observationEvent.onoxygen == true)
            {
                ewsResponse.score += 2;
            }


            //Systolic Blood Pressure
            // Requires Units to be present as this calculation is based on mmHg.
            if (ValidationHelper.IsValidWholeNumber((Double)observationEvent.bps))
            {
                if (observationEvent.bps <= 90)
                {
                    ewsResponse.score += 3;
                    redScoreIndicator = true;
                }
                else if (observationEvent.bps >= 91 && observationEvent.bps <= 100)
                {
                    ewsResponse.score += 2;
                }
                else if (observationEvent.bps >= 101 && observationEvent.bps <= 110)
                {
                    ewsResponse.score += 1;
                }
                else if (observationEvent.bps >= 220)
                {
                    ewsResponse.score += 3;
                    redScoreIndicator = true;
                }

            }
            else
            {
                //
                ewsResponse.status = "ERROR";
                ewsResponse.error = "No valid Systolic BP value has been recorded in this observation event id.";
                ewsResponse.score = null;
                return ewsResponse;
            }

            //Pulse
            if (ValidationHelper.IsValidWholeNumber((Double)observationEvent.pulse))
            {
                if (observationEvent.pulse <= 40)
                {
                    ewsResponse.score += 3;
                    redScoreIndicator = true;
                }
                else if (observationEvent.pulse >= 41 && observationEvent.pulse <= 50)
                {
                    ewsResponse.score += 1;
                }
                else if (observationEvent.pulse >= 91 && observationEvent.pulse <= 110)
                {
                    ewsResponse.score += 1;
                }
                else if (observationEvent.pulse >= 111 && observationEvent.pulse <= 130)
                {
                    ewsResponse.score += 2;
                }
                else if (observationEvent.pulse >= 131)
                {
                    ewsResponse.score += 3;
                    redScoreIndicator = true;
                }

            }
            else
            {
                ewsResponse.status = "ERROR";
                ewsResponse.error = "No valid Pulse value has been recorded in this observation event id.";
                ewsResponse.score = null;
                return ewsResponse;
            }


            //Conscious Level
            if (String.IsNullOrEmpty(observationEvent.acvpu.ToString()) == false)
            {
                if (observationEvent.acvpu.ToUpper() == "A")
                {
                    //do nothing
                }
                else if (observationEvent.acvpu.ToUpper() == "C")
                {
                    ewsResponse.score += 3;
                    redScoreIndicator = true;
                }
                else if (observationEvent.acvpu.ToUpper() == "V")
                {
                    ewsResponse.score += 3;
                    redScoreIndicator = true;
                }
                else if (observationEvent.acvpu.ToUpper() == "P")
                {
                    ewsResponse.score += 3;
                    redScoreIndicator = true;
                }
                else if (observationEvent.acvpu.ToUpper() == "U")
                {
                    ewsResponse.score += 3;
                    redScoreIndicator = true;
                }
                else
                {
                    ewsResponse.status = "ERROR";
                    ewsResponse.error = "No valid Conscious Level value has been recorded in this observation event id.";
                    ewsResponse.score = null;
                    return ewsResponse;
                }
            }
            else
            {
                ewsResponse.status = "ERROR";
                ewsResponse.error = "No valid Conscious Level value has been recorded in this observation event id.";
                ewsResponse.score = null;
                return ewsResponse;
            }

            if (String.IsNullOrEmpty(observationEvent.temp.ToString()) == false)
            {
                if (observationEvent.temp <= 35.0)
                {
                    ewsResponse.score += 3;
                    redScoreIndicator = true;
                }
                else if (observationEvent.temp >= 35.1 && observationEvent.temp <= 36.0)
                {
                    ewsResponse.score += 1;
                }
                else if (observationEvent.temp >= 38.1 && observationEvent.temp <= 39.0)
                {
                    ewsResponse.score += 1;
                }
                else if (observationEvent.temp >= 39.1)
                {
                    ewsResponse.score += 2;
                }

            }
            else
            {
                ewsResponse.status = "ERROR";
                ewsResponse.error = "No valid Temp value has been recorded in this observation event id.";
                ewsResponse.score = null;
                return ewsResponse;
            }

            // Add guidance
            if (ewsResponse.score >= 7)
            {
                ewsResponse.guidance = "HIGH CLINICAL RISK - URGENT OR EMERGENCY RESPONSE!";
            }
            else if (ewsResponse.score >= 5 && ewsResponse.score <= 6)
            {
                ewsResponse.guidance = "MEDIUM CLINICAL RISK - KEY THRESHOLD FOR URGENT RESPONSE!";
            }
            else if (ewsResponse.score < 5 && redScoreIndicator == true)
            {
                ewsResponse.guidance = "LOW/MEDIUM CLINICAL RISK - URGENT WARD-BASED RESPONSE!";
            }
            else if (ewsResponse.score >= 0 && ewsResponse.score <= 4 && redScoreIndicator == false)
            {
                ewsResponse.guidance = "LOW CLINICAL RISK - WARD-BASED RESPONSE!";
            }

            // Finished calculating
            if (String.IsNullOrEmpty(ewsResponse.error) == true)
            {
                ewsResponse.status = "SUCCESS";
            }


            return ewsResponse;
            
        }
    }

}