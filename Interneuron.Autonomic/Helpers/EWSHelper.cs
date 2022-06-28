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

namespace Interneuron.Autonomic.Helpers
{
    public class EWSHelper
    {

        public static EWSResponseScoreParameter CreateScoreParam(string _name, int _score, string _value)
        {
            EWSResponseScoreParameter ewsParam = new EWSResponseScoreParameter();

            ewsParam.name = _name;
            ewsParam.score = _score;
            ewsParam.value = _value;

            return ewsParam;

        }

        public static List<ScoreParameter> CopyEWSScoreParameterList(List<EWSResponseScoreParameter> ewsParamList, string score_id, string observationevent_id)
        {
            List<ScoreParameter> scoreParamList = new List<ScoreParameter>();

            foreach (EWSResponseScoreParameter ewsScoreParam in ewsParamList)
            {

                ScoreParameter param = new ScoreParameter();

                param.scoreparameter_id = Guid.NewGuid().ToString(); //TODO: Need to check if already exists (GetParameterScoreId
                param.score_id = score_id;
                param.observationevent_id = observationevent_id;

                param.parameter = ewsScoreParam.name;
                param.score = ewsScoreParam.score;
                param.value = ewsScoreParam.value;

                // add the transformed score to the list
                scoreParamList.Add(param);

            }

            return scoreParamList;


        }

    }
}
