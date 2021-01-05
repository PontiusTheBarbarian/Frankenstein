using System.Collections.Generic;
using Frankenstein.Models;

namespace Frankenstein.Extensions
{
	public static class StringExtensions
	{
		public static string SubstituteVariables(this string input, List<Variable> variables)
		{
			foreach(var variable in variables)
			{
				if (input.Contains(variable.AsPowershellVariable()))
				{
					input = input.Replace(variable.AsPowershellVariable(), variable.Value);
				}
			}

			return input;
		}
	}
}
