/***************************************************************************
 *                                  Main.cs
 *                            -------------------
 *   begin                : May 1, 2002
 *   copyright            : (C) The RunUO Software Team
 *   email                : info@runuo.com
 *
 *   $Id: Main.cs 4 2006-06-15 04:28:39Z mark $
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
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using Server;
using Server.Accounting;
using Server.Gumps;
using Server.Network;

namespace Server {
	public delegate void Slice();

	public class Core {
		private static bool m_Crashed;
		private static Thread timerThread;
		private static string m_BaseDirectory;
		private static string m_ExePath;
		private static List<string> m_DataDirectories = new List<string>();
		private static Assembly m_Assembly;
		private static Process m_Process;
		private static Thread m_Thread;
		private static bool m_Service;
		private static bool m_Debug;
		private static MultiTextWriter m_MultiConOut;

		private static bool m_Profiling;
		private static DateTime m_ProfileStart;
		private static TimeSpan m_ProfileTime;

		private static MessagePump m_MessagePump;

		public static MessagePump MessagePump {
			get { return m_MessagePump; }
			set { m_MessagePump = value; }
		}

		public static Slice Slice;

		public static bool Profiling {
			get { return m_Profiling; }
			set {
				if ( m_Profiling == value )
					return;

				m_Profiling = value;

				if ( m_ProfileStart > DateTime.MinValue )
					m_ProfileTime += DateTime.Now - m_ProfileStart;

				m_ProfileStart = ( m_Profiling ? DateTime.Now : DateTime.MinValue );
			}
		}

		public static TimeSpan ProfileTime {
			get {
				if ( m_ProfileStart > DateTime.MinValue )
					return m_ProfileTime + ( DateTime.Now - m_ProfileStart );

				return m_ProfileTime;
			}
		}

		public static bool Service { get { return m_Service; } }
		public static bool Debug { get { return m_Debug; } }
		public static List<string> DataDirectories { get { return m_DataDirectories; } }
		public static Assembly Assembly { get { return m_Assembly; } set { m_Assembly = value; } }
		public static Process Process { get { return m_Process; } }
		public static Thread Thread { get { return m_Thread; } }
		public static MultiTextWriter MultiConsoleOut { get { return m_MultiConOut; } }

		public static readonly bool Is64Bit = ( IntPtr.Size == 8 );

		private static bool m_MultiProcessor;
		private static int m_ProcessorCount;

		public static bool MultiProcessor { get { return m_MultiProcessor; } set { m_MultiProcessor = value; } }
		public static int ProcessorCount { get { return m_ProcessorCount; } set { m_ProcessorCount = value; } }

		public static string FindDataFile( string path ) {
			if ( m_DataDirectories.Count == 0 )
				throw new InvalidOperationException( "Attempted to FindDataFile before DataDirectories list has been filled." );

			string fullPath = null;

			for ( int i = 0; i < m_DataDirectories.Count; ++i ) {
				fullPath = Path.Combine( m_DataDirectories[i], path );

				if ( File.Exists( fullPath ) )
					break;

				fullPath = null;
			}

			return fullPath;
		}

		public static string FindDataFile( string format, params object[] args ) {
			return FindDataFile( String.Format( format, args ) );
		}

		#region Expansions

		private static Expansion m_Expansion;
		public static Expansion Expansion {
			get { return m_Expansion; }
			set { m_Expansion = value; }
		}

		public static bool AOS {
			get {
				return m_Expansion >= Expansion.AOS;
			}
		}

		public static bool SE {
			get {
				return m_Expansion >= Expansion.SE;
			}
		}

		public static bool ML {
			get {
				return m_Expansion >= Expansion.ML;
			}
		}

		#endregion

		public static string ExePath {
			get {
				if ( m_ExePath == null ) {
					m_ExePath = Assembly.Location;
					//m_ExePath = Process.GetCurrentProcess().MainModule.FileName;
				}

				return m_ExePath;
			}
		}

		public static string BaseDirectory {
			get {
				if ( m_BaseDirectory == null ) {
					try {
						m_BaseDirectory = ExePath;

						if ( m_BaseDirectory.Length > 0 )
							m_BaseDirectory = Path.GetDirectoryName( m_BaseDirectory );
					} catch {
						m_BaseDirectory = "";
					}
				}

				return m_BaseDirectory;
			}
		}

		private static void CurrentDomain_UnhandledException( object sender, UnhandledExceptionEventArgs e ) {
			Console.WriteLine( e.IsTerminating ? "Error:" : "Warning:" );
			Console.WriteLine( e.ExceptionObject );

			if ( e.IsTerminating ) {
				m_Crashed = true;

				bool close = false;

				try {
					CrashedEventArgs args = new CrashedEventArgs( e.ExceptionObject as Exception );

					EventSink.InvokeCrashed( args );

					close = args.Close;
				} catch {
				}

				if ( !close && !m_Service ) {
					try {
						for ( int i = 0; i < m_MessagePump.Listeners.Length; i++ ) {
							m_MessagePump.Listeners[i].Dispose();
						}
					} catch {
					}

					Console.WriteLine( "This exception is fatal, press return to exit" );
					Console.ReadLine();
				}

				m_Closing = true;
			}
		}

#if !MONO
		private enum ConsoleEventType {
			CTRL_C_EVENT,
			CTRL_BREAK_EVENT,
			CTRL_CLOSE_EVENT,
			CTRL_LOGOFF_EVENT = 5,
			CTRL_SHUTDOWN_EVENT
		}

		private delegate bool ConsoleEventHandler( ConsoleEventType type );
		private static ConsoleEventHandler m_ConsoleEventHandler;

		[System.Runtime.InteropServices.DllImport( "Kernel32.dll" )]
		private static extern bool SetConsoleCtrlHandler( ConsoleEventHandler callback, bool add );

		private static bool OnConsoleEvent( ConsoleEventType type ) {
			if ( World.Saving )
				return true;

			return false;
		}
#endif

		private static void CurrentDomain_ProcessExit( object sender, EventArgs e ) {
			HandleClosed();
		}

		private static bool m_Closing;

		public static bool Closing { get { return m_Closing; } set { m_Closing = value; } }

		private static int m_CycleIndex;
		private static float[] m_CyclesPerSecond = new float[100];

		public static float CyclesPerSecond {
			get { return m_CyclesPerSecond[Math.Max( 0, ( m_CycleIndex - 1 ) % m_CyclesPerSecond.Length )]; }
		}

		public static float AverageCPS {
			get {
				float t = 0.0f;
				int c = 0;

				for ( int i = 0; i < m_CycleIndex && i < m_CyclesPerSecond.Length; ++i ) {
					t += m_CyclesPerSecond[i];
					++c;
				}

				return ( t / Math.Max( c, 1 ) );
			}
		}

		private static void HandleClosed() {
			if ( m_Closing )
				return;

			m_Closing = true;

			Console.Write( "Exiting..." );

			if ( !m_Crashed )
				EventSink.InvokeShutdown( new ShutdownEventArgs() );

			if ( timerThread.ThreadState != System.Threading.ThreadState.Unstarted )
				timerThread.Join();

			Console.WriteLine( "done" );
		}

		public static void Main( string[] args ) {
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler( CurrentDomain_UnhandledException );
			AppDomain.CurrentDomain.ProcessExit += new EventHandler( CurrentDomain_ProcessExit );

			for ( int i = 0; i < args.Length; ++i ) {
				if ( Insensitive.Equals( args[i], "-debug" ) )
					m_Debug = true;
				else if ( Insensitive.Equals( args[i], "-service" ) )
					m_Service = true;
				else if ( Insensitive.Equals( args[i], "-profile" ) )
					Profiling = true;
			}

			try {
				if ( m_Service ) {
					if ( !Directory.Exists( "Logs" ) )
						Directory.CreateDirectory( "Logs" );

					Console.SetOut( m_MultiConOut = new MultiTextWriter( Console.Out, new FileLogger( "Logs/Console.log" ) ) );
				} else {
					Console.SetOut( m_MultiConOut = new MultiTextWriter( Console.Out ) );
				}
			} catch {
			}

#if !MONO
			m_ConsoleEventHandler = new ConsoleEventHandler( OnConsoleEvent );
			SetConsoleCtrlHandler( m_ConsoleEventHandler, true );
#endif

			m_Thread = Thread.CurrentThread;
			m_Process = Process.GetCurrentProcess();
			m_Assembly = Assembly.GetEntryAssembly();

			if ( m_Thread != null )
				m_Thread.Name = "Core Thread";

			if ( BaseDirectory.Length > 0 )
				Directory.SetCurrentDirectory( BaseDirectory );

			Timer.TimerThread ttObj = new Timer.TimerThread();
			timerThread = new Thread( new ThreadStart( ttObj.TimerMain ) );
			timerThread.Name = "Timer Thread";

			Version ver = m_Assembly.GetName().Version;

			// Added to help future code support on forums, as a 'check' people can ask for to it see if they recompiled core or not
			Console.WriteLine( "Version: Khaeros Build 10/8/2010 4:00 PM EST" );
			Console.WriteLine( "--------------------------------------------" );
			Console.WriteLine( "Using: RunUO Version {0}.{1}, Build {2}.{3}", ver.Major, ver.Minor, ver.Build, ver.Revision );
			Console.WriteLine( "Running: .NET Framework Version {0}.{1}.{2}", Environment.Version.Major, Environment.Version.Minor, Environment.Version.Build );

			string s = Arguments;

			if ( s.Length > 0 )
				Console.WriteLine( "Core: Running with arguments: {0}", s );

			int processorCount = Utility.ToInt32( Environment.GetEnvironmentVariable( "NUMBER_OF_PROCESSORS" ) );

			if ( processorCount > 1 )
				m_MultiProcessor = true;

			if ( processorCount > 1 || Is64Bit )
				Console.WriteLine( "Core: Optimizing for {0} {2}processor{1}", processorCount, processorCount == 1 ? "" : "s", Is64Bit ? "64-bit " : "" );

			m_ProcessorCount = processorCount;

			while ( !ScriptCompiler.Compile( m_Debug ) ) {
				Console.WriteLine( "Scripts: One or more scripts failed to compile or no script files were found." );
				Console.WriteLine( " - Press return to exit, or R to try again." );

				if ( Console.ReadKey( true ).Key != ConsoleKey.R )
					return;
			}

			MessagePump ms = m_MessagePump = new MessagePump( new Listener( Listener.Port ) );

			timerThread.Start();

			for ( int i = 0; i < Map.AllMaps.Count; ++i )
				Map.AllMaps[i].Tiles.Force();

			NetState.Initialize();

			EventSink.InvokeServerStarted();

			try {
				DateTime now, last = DateTime.Now;

				const int sampleInterval = 100;
				const float ticksPerSecond = ( float ) ( TimeSpan.TicksPerSecond * sampleInterval );

				int sample = 0;

				TimeSpan multiCoreSleep = new TimeSpan( 10000 );
                TimeSpan singleCoreSleep = new TimeSpan( 1000000 );

                while ( !m_Closing ) {
                    Thread.Sleep( m_MultiProcessor ? multiCoreSleep : singleCoreSleep );

					Mobile.ProcessDeltaQueue();
					Item.ProcessDeltaQueue();

					Timer.Slice();
					m_MessagePump.Slice();

					NetState.FlushAll();
					NetState.ProcessDisposedQueue();

					if ( Slice != null )
						Slice();

					if ( ( ++sample % sampleInterval ) == 0 ) {
						now = DateTime.Now;
						m_CyclesPerSecond[m_CycleIndex++ % m_CyclesPerSecond.Length] =
							ticksPerSecond / ( now.Ticks - last.Ticks );
						last = now;
					}
				}
			} catch ( Exception e ) {
				CurrentDomain_UnhandledException( null, new UnhandledExceptionEventArgs( e, true ) );
			}

			if ( timerThread.IsAlive )
				timerThread.Abort();
		}

		public static string Arguments {
			get {
				StringBuilder sb = new StringBuilder();

				if ( Core.Debug )
					Utility.Seperate( sb, "-debug", " " );

				if ( Core.Service )
					Utility.Seperate( sb, "-service", " " );

				if ( Core.Profiling )
					Utility.Seperate( sb, "-profile", " " );

				return sb.ToString();
			}
		}

		private static int m_GlobalMaxUpdateRange = 24;

		public static int GlobalMaxUpdateRange {
			get { return m_GlobalMaxUpdateRange; }
			set { m_GlobalMaxUpdateRange = value; }
		}

		private static int m_ItemCount, m_MobileCount;

		public static int ScriptItems { get { return m_ItemCount; } }
		public static int ScriptMobiles { get { return m_MobileCount; } }

		public static void VerifySerialization() {
			m_ItemCount = 0;
			m_MobileCount = 0;

			VerifySerialization( Assembly.GetCallingAssembly() );

			for ( int a = 0; a < ScriptCompiler.Assemblies.Length; ++a )
				VerifySerialization( ScriptCompiler.Assemblies[a] );
		}

		private static void VerifySerialization( Assembly a ) {
			if ( a == null )
				return;

			Type[] ctorTypes = new Type[] { typeof( Serial ) };

			foreach ( Type t in a.GetTypes() ) {
				bool isItem = t.IsSubclassOf( typeof( Item ) );

				if ( isItem || t.IsSubclassOf( typeof( Mobile ) ) ) {
					if ( isItem )
						++m_ItemCount;
					else
						++m_MobileCount;

					bool warned = false;

					try {

						/*
						if( isItem && t.IsPublic && !t.IsAbstract )
						{
							ConstructorInfo cInfo = t.GetConstructor( Type.EmptyTypes );
							if( cInfo == null )
							{
								if( !warned )
									Console.WriteLine( "Warning: {0}", t );

								warned = true;
								Console.WriteLine( "       - No zero paramater constructor" );
							}
						}
						*/

						if ( t.GetConstructor( ctorTypes ) == null ) {
							if ( !warned )
								Console.WriteLine( "Warning: {0}", t );

							warned = true;
							Console.WriteLine( "       - No serialization constructor" );
						}

						if ( t.GetMethod( "Serialize", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly ) == null ) {
							if ( !warned )
								Console.WriteLine( "Warning: {0}", t );

							warned = true;
							Console.WriteLine( "       - No Serialize() method" );
						}

						if ( t.GetMethod( "Deserialize", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly ) == null ) {
							if ( !warned )
								Console.WriteLine( "Warning: {0}", t );

							warned = true;
							Console.WriteLine( "       - No Deserialize() method" );
						}

						if ( warned )
							Console.WriteLine();
					} catch {
					}
				}
			}
		}
	}

	public class FileLogger : TextWriter, IDisposable {
		private string m_FileName;
		private bool m_NewLine;
		public const string DateFormat = "[MMMM dd hh:mm:ss.f tt]: ";

		public string FileName { get { return m_FileName; } }

		public FileLogger( string file )
			: this( file, false ) {
		}

		public FileLogger( string file, bool append ) {
			m_FileName = file;
			using ( StreamWriter writer = new StreamWriter( new FileStream( m_FileName, append ? FileMode.Append : FileMode.Create, FileAccess.Write, FileShare.Read ) ) ) {
				writer.WriteLine( ">>>Logging started on {0}.", DateTime.Now.ToString( "f" ) ); //f = Tuesday, April 10, 2001 3:51 PM 
			}
			m_NewLine = true;
		}

		public override void Write( char ch ) {
			using ( StreamWriter writer = new StreamWriter( new FileStream( m_FileName, FileMode.Append, FileAccess.Write, FileShare.Read ) ) ) {
				if ( m_NewLine ) {
					writer.Write( DateTime.Now.ToString( DateFormat ) );
					m_NewLine = false;
				}
				writer.Write( ch );
			}
		}

		public override void Write( string str ) {
			using ( StreamWriter writer = new StreamWriter( new FileStream( m_FileName, FileMode.Append, FileAccess.Write, FileShare.Read ) ) ) {
				if ( m_NewLine ) {
					writer.Write( DateTime.Now.ToString( DateFormat ) );
					m_NewLine = false;
				}
				writer.Write( str );
			}
		}

		public override void WriteLine( string line ) {
			using ( StreamWriter writer = new StreamWriter( new FileStream( m_FileName, FileMode.Append, FileAccess.Write, FileShare.Read ) ) ) {
				if ( m_NewLine )
					writer.Write( DateTime.Now.ToString( DateFormat ) );
				writer.WriteLine( line );
				m_NewLine = true;
			}
		}

		public override System.Text.Encoding Encoding {
			get { return System.Text.Encoding.Default; }
		}
	}

	public class MultiTextWriter : TextWriter {
		private List<TextWriter> m_Streams;

		public MultiTextWriter( params TextWriter[] streams ) {
			m_Streams = new List<TextWriter>( streams );

			if ( m_Streams.Count < 0 )
				throw new ArgumentException( "You must specify at least one stream." );
		}

		public void Add( TextWriter tw ) {
			m_Streams.Add( tw );
		}

		public void Remove( TextWriter tw ) {
			m_Streams.Remove( tw );
		}

		public override void Write( char ch ) {
			for ( int i = 0; i < m_Streams.Count; i++ )
				m_Streams[i].Write( ch );
		}

		public override void WriteLine( string line ) {
			for ( int i = 0; i < m_Streams.Count; i++ )
				m_Streams[i].WriteLine( line );
		}

		public override void WriteLine( string line, params object[] args ) {
			WriteLine( String.Format( line, args ) );
		}

		public override Encoding Encoding {
			get { return Encoding.Default; }
		}
	}
}
