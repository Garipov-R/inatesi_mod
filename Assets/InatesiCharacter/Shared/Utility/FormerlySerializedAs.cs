// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// Opsive.Shared.Utility.FormerlySerializedAs

using System;

namespace InatesiCharacter.Shared.Utility
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class FormerlySerializedAs : Attribute
	{
		private string m_FullName;

		public string FullName => m_FullName;

		public FormerlySerializedAs(string fullName)
		{
			m_FullName = fullName;
		}
	}
}

