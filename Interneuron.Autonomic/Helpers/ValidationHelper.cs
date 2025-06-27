//BEGIN LICENSE BLOCK 
//Interneuron Autonomic

//Copyright(C) 2025  Interneuron Limited

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
﻿//Interneuron Synapse

//Copyright(C) 2023  Interneuron Holdings Ltd

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

namespace Interneuron.Autonomic.Helpers
{
    public class ValidationHelper
    {
        // Generic helper function to check if a provided Double is whole number
        // and is not null, blank or empty.
        public static bool IsValidWholeNumber(Double doubleToVerify)
        {

            if (String.IsNullOrEmpty(doubleToVerify.ToString()))
            {
                return false;
            }

            if ((doubleToVerify == Math.Floor(doubleToVerify)) && !Double.IsInfinity(doubleToVerify))
            {
                // integer type
            }
            else
            {
                return false;
            }


            return true;
        }


    }
}
