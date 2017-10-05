/***************************************************************************
 *                               Attributes.cs
 *                            -------------------
 *   begin                : May 1, 2002
 *   copyright            : (C) The RunUO Software Team
 *   email                : info@runuo.com
 *
 *   $Id: Attributes.cs 4 2006-06-15 04:28:39Z mark $
 *
 ***************************************************************************/

/***************************************************************************
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 2 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Server
{
	[AttributeUsage( AttributeTargets.Property )]
	public class HueAttribute : Attribute
	{
		public HueAttribute()
		{
		}
	}

	[AttributeUsage( AttributeTargets.Property )]
	public class BodyAttribute : Attribute
	{
		public BodyAttribute()
		{
		}
	}

	[AttributeUsage( AttributeTargets.Class | AttributeTargets.Struct )]
	public class PropertyObjectAttribute : Attribute
	{
		public PropertyObjectAttribute()
		{
		}
	}

	[AttributeUsage( AttributeTargets.Class | AttributeTargets.Struct )]
	public class NoSortAttribute : Attribute
	{
		public NoSortAttribute()
		{
		}
	}

	[AttributeUsage( AttributeTargets.Method )]
	public class CallPriorityAttribute : Attribute
	{
		private int m_Priority;

		public int Priority
		{
			get{ return m_Priority; }
			set{ m_Priority = value; }
		}

		public CallPriorityAttribute( int priority )
		{
			m_Priority = priority;
		}
	}

	public class CallPriorityComparer : IComparer<MethodInfo>
	{
		public int Compare( MethodInfo x, MethodInfo y )
		{
			if ( x == null && y == null )
				return 0;

			if ( x == null )
				return 1;

			if ( y == null )
				return -1;

			return GetPriority( x ) - GetPriority( y );
		}

		private int GetPriority( MethodInfo mi )
		{
			object[] objs = mi.GetCustomAttributes( typeof( CallPriorityAttribute ), true );

			if ( objs == null )
				return 0;

			if ( objs.Length == 0 )
				return 0;

			CallPriorityAttribute attr = objs[0] as CallPriorityAttribute;

			if ( attr == null )
				return 0;

			return attr.Priority;
		}
	}

	[AttributeUsage( AttributeTargets.Class )]
	public class TypeAliasAttribute : Attribute
	{
		private string[] m_Aliases;

		public string[] Aliases
		{
			get
			{
				return m_Aliases;
			}
		}

		public TypeAliasAttribute( params string[] aliases )
		{
			m_Aliases = aliases;
		}
	}

	[AttributeUsage( AttributeTargets.Class | AttributeTargets.Struct )]
	public class ParsableAttribute : Attribute
	{
		public ParsableAttribute()
		{
		}
	}

	[AttributeUsage( AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum )]
	public class CustomEnumAttribute : Attribute
	{
		private string[] m_Names;

		public string[] Names
		{
			get
			{
				return m_Names;
			}
		}

		public CustomEnumAttribute( string[] names )
		{
			m_Names = names;
		}
	}

	[AttributeUsage( AttributeTargets.Constructor )]
	public class ConstructableAttribute : Attribute
	{
		public ConstructableAttribute()
		{
		}
	}

	[AttributeUsage( AttributeTargets.Property )]
	public class CommandPropertyAttribute : Attribute
	{
		private AccessLevel m_ReadLevel, m_WriteLevel;

		public AccessLevel ReadLevel
		{
			get
			{
				return m_ReadLevel;
			}
		}

		public AccessLevel WriteLevel
		{
			get
			{
				return m_WriteLevel;
			}
		}

		public CommandPropertyAttribute( AccessLevel level ) : this( level, level )
		{
		}

		public CommandPropertyAttribute( AccessLevel readLevel, AccessLevel writeLevel )
		{
			m_ReadLevel = readLevel;
			m_WriteLevel = writeLevel;
		}
	}
}