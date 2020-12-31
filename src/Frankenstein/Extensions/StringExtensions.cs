using System.Collections.Generic;
using Frankenstein.Models;

namespace Frankenstein.Extensions
{
	public static class StringExtensions
	{
		public static List<Variable> Variables { get; set; }

		public static string SubstituteVariables(this string input)
		{
			foreach(var variable in Variables)
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
