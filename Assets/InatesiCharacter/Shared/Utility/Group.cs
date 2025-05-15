using System;

namespace InatesiCharacter.Shared.Utility
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class Group : Attribute
	{
		private string m_Name;

		public string Name => m_Name;

		public Group(string name)
		{
			m_Name = name;
		}
	}
}
