#region Apache License
//
// Licensed to the Apache Software Foundation (ASF) under one or more 
// contributor license agreements. See the NOTICE file distributed with
// this work for additional information regarding copyright ownership. 
// The ASF licenses this file to you under the Apache License, Version 2.0
// (the "License"); you may not use this file except in compliance with 
// the License. You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion

using System;
using System.Collections;

namespace Log4Net.Core
{
  /// <summary>
  /// Defines the default set of levels recognized by the system.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Each <see cref="LoggingEvent"/> has an associated <see cref="Level"/>.
  /// </para>
  /// <para>
  /// Levels have a numeric <see cref="Level.Value"/> that defines the relative 
  /// ordering between levels. Two Levels with the same <see cref="Level.Value"/> 
  /// are deemed to be equivalent.
  /// </para>
  /// <para>
  /// The levels that are recognized by log4net are set for each <see cref="Log4Net.Repository.ILoggerRepository"/>
  /// and each repository can have different levels defined. The levels are stored
  /// in the <see cref="Log4Net.Repository.ILoggerRepository.LevelMap"/> on the repository. Levels are
  /// looked up by name from the <see cref="Log4Net.Repository.ILoggerRepository.LevelMap"/>.
  /// </para>
  /// <para>
  /// When logging at level INFO the actual level used is not <see cref="Level.Info"/> but
  /// the value of <c>LoggerRepository.LevelMap["INFO"]</c>. The default value for this is
  /// <see cref="Level.Info"/>, but this can be changed by reconfiguring the level map.
  /// </para>
  /// <para>
  /// Each level has a <see cref="DisplayName"/> in addition to its <see cref="Name"/>. The 
  /// <see cref="DisplayName"/> is the string that is written into the output log. By default
  /// the display name is the same as the level name, but this can be used to alias levels
  /// or to localize the log output.
  /// </para>
  /// <para>
  /// Some of the predefined levels recognized by the system are:
  /// </para>
  /// <list type="bullet">
  ///		<item>
  ///			<description><see cref="Off"/>.</description>
  ///		</item>
  ///		<item>
  ///			<description><see cref="Fatal"/>.</description>
  ///		</item>
  ///		<item>
  ///			<description><see cref="Error"/>.</description>
  ///		</item>
  ///		<item>
  ///			<description><see cref="Warn"/>.</description>
  ///		</item>
  ///		<item>
  ///			<description><see cref="Info"/>.</description>
  ///		</item>
  ///		<item>
  ///			<description><see cref="Debug"/>.</description>
  ///		</item>
  ///		<item>
  ///			<description><see cref="All"/>.</description>
  ///		</item>
  /// </list>
  /// </remarks>
  /// <author>Nicko Cadell</author>
  /// <author>Gert Driesen</author>
#if !NETCF
  [Serializable]
#endif
  sealed public class Level : IComparable
  {
    #region Private Instance Fields

    private readonly int mLevelValue;
    private readonly string mLevelName;
    private readonly string mLevelDisplayName;

    #endregion Private Instance Fields

    #region Public Constants

    public const int LevelValueOff = int.MaxValue;
    public const int LevelValueLog4Net_Debug = 120000;
    public const int LevelValueEmergency = 120000;
    public const int LevelValueFatal = 110000;
    public const int LevelValueAlert = 100000;
    public const int LevelValueCritical = 90000;
    public const int LevelValueSevere = 80000;
    public const int LevelValueError = 70000;
    public const int LevelValueWarn = 60000;
    public const int LevelValueNotice = 50000;
    public const int LevelValueInfo = 40000;
    public const int LevelValueDebug = 30000;
    public const int LevelValueFine = 30000;
    public const int LevelValueTrace = 20000;
    public const int LevelValueFiner = 20000;
    public const int LevelValueVerbose = 10000;
    public const int LevelValueFinest = 10000;
    public const int LevelValueAll = int.MinValue;
    
    #endregion Public Constants

    #region Public Instance Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="level">Integer value for this level, higher values represent more severe levels.</param>
    /// <param name="levelName">The string name of this level.</param>
    /// <param name="displayName">The display name for this level. This may be localized or otherwise different from the name</param>
    /// <remarks>
    /// <para>
    /// Initializes a new instance of the <see cref="Level" /> class with
    /// the specified level name and value.
    /// </para>
    /// </remarks>
    public Level(int level, string levelName, string displayName)
    {
      if (levelName == null)
      {
        throw new ArgumentNullException("levelName");
      }
      if (displayName == null)
      {
        throw new ArgumentNullException("displayName");
      }

      mLevelValue = level;
      mLevelName = string.Intern(levelName);
      mLevelDisplayName = displayName;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="level">Integer value for this level, higher values represent more severe levels.</param>
    /// <param name="levelName">The string name of this level.</param>
    /// <remarks>
    /// <para>
    /// Initializes a new instance of the <see cref="Level" /> class with
    /// the specified level name and value.
    /// </para>
    /// </remarks>
    public Level(int level, string levelName)
      : this(level, levelName, levelName)
    {
    }

    #endregion Public Instance Constructors

    #region Public Instance Properties

    /// <summary>
    /// Gets the name of this level.
    /// </summary>
    /// <value>
    /// The name of this level.
    /// </value>
    /// <remarks>
    /// <para>
    /// Gets the name of this level.
    /// </para>
    /// </remarks>
    public string Name
    {
      get { return mLevelName; }
    }

    /// <summary>
    /// Gets the value of this level.
    /// </summary>
    /// <value>
    /// The value of this level.
    /// </value>
    /// <remarks>
    /// <para>
    /// Gets the value of this level.
    /// </para>
    /// </remarks>
    public int Value
    {
      get { return mLevelValue; }
    }

    /// <summary>
    /// Gets the display name of this level.
    /// </summary>
    /// <value>
    /// The display name of this level.
    /// </value>
    /// <remarks>
    /// <para>
    /// Gets the display name of this level.
    /// </para>
    /// </remarks>
    public string DisplayName
    {
      get { return mLevelDisplayName; }
    }

    #endregion Public Instance Properties

    #region Override implementation of Object

    /// <summary>
    /// Returns the <see cref="string" /> representation of the current 
    /// <see cref="Level" />.
    /// </summary>
    /// <returns>
    /// A <see cref="string" /> representation of the current <see cref="Level" />.
    /// </returns>
    /// <remarks>
    /// <para>
    /// Returns the level <see cref="Name"/>.
    /// </para>
    /// </remarks>
    override public string ToString()
    {
      return mLevelName;
    }

    /// <summary>
    /// Compares levels.
    /// </summary>
    /// <param name="o">The object to compare against.</param>
    /// <returns><c>true</c> if the objects are equal.</returns>
    /// <remarks>
    /// <para>
    /// Compares the levels of <see cref="Level" /> instances, and 
    /// defers to base class if the target object is not a <see cref="Level" />
    /// instance.
    /// </para>
    /// </remarks>
    override public bool Equals(object o)
    {
      Level otherLevel = o as Level;
      if (otherLevel != null)
      {
        return mLevelValue == otherLevel.mLevelValue;
      }
      else
      {
        return base.Equals(o);
      }
    }

    /// <summary>
    /// Returns a hash code
    /// </summary>
    /// <returns>A hash code for the current <see cref="Level" />.</returns>
    /// <remarks>
    /// <para>
    /// Returns a hash code suitable for use in hashing algorithms and data 
    /// structures like a hash table.
    /// </para>
    /// <para>
    /// Returns the hash code of the level <see cref="Value"/>.
    /// </para>
    /// </remarks>
    override public int GetHashCode()
    {
      return mLevelValue;
    }

    #endregion Override implementation of Object

    #region Implementation of IComparable

    /// <summary>
    /// Compares this instance to a specified object and returns an 
    /// indication of their relative values.
    /// </summary>
    /// <param name="r">A <see cref="Level"/> instance or <see langword="null" /> to compare with this instance.</param>
    /// <returns>
    /// A 32-bit signed integer that indicates the relative order of the 
    /// values compared. The return value has these meanings:
    /// <list type="table">
    ///		<listheader>
    ///			<term>Value</term>
    ///			<description>Meaning</description>
    ///		</listheader>
    ///		<item>
    ///			<term>Less than zero</term>
    ///			<description>This instance is less than <paramref name="r" />.</description>
    ///		</item>
    ///		<item>
    ///			<term>Zero</term>
    ///			<description>This instance is equal to <paramref name="r" />.</description>
    ///		</item>
    ///		<item>
    ///			<term>Greater than zero</term>
    ///			<description>
    ///				<para>This instance is greater than <paramref name="r" />.</para>
    ///				<para>-or-</para>
    ///				<para><paramref name="r" /> is <see langword="null" />.</para>
    ///				</description>
    ///		</item>
    /// </list>
    /// </returns>
    /// <remarks>
    /// <para>
    /// <paramref name="r" /> must be an instance of <see cref="Level" /> 
    /// or <see langword="null" />; otherwise, an exception is thrown.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentException"><paramref name="r" /> is not a <see cref="Level" />.</exception>
    public int CompareTo(object r)
    {
      Level target = r as Level;
      if (target != null)
      {
        return Compare(this, target);
      }
      throw new ArgumentException("Parameter: r, Value: [" + r + "] is not an instance of Level");
    }

    #endregion Implementation of IComparable

    #region Operators

    /// <summary>
    /// Returns a value indicating whether a specified <see cref="Level" /> 
    /// is greater than another specified <see cref="Level" />.
    /// </summary>
    /// <param name="l">A <see cref="Level" /></param>
    /// <param name="r">A <see cref="Level" /></param>
    /// <returns>
    /// <c>true</c> if <paramref name="l" /> is greater than 
    /// <paramref name="r" />; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// <para>
    /// Compares two levels.
    /// </para>
    /// </remarks>
    public static bool operator >(Level l, Level r)
    {
      return l.mLevelValue > r.mLevelValue;
    }

    /// <summary>
    /// Returns a value indicating whether a specified <see cref="Level" /> 
    /// is less than another specified <see cref="Level" />.
    /// </summary>
    /// <param name="l">A <see cref="Level" /></param>
    /// <param name="r">A <see cref="Level" /></param>
    /// <returns>
    /// <c>true</c> if <paramref name="l" /> is less than 
    /// <paramref name="r" />; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// <para>
    /// Compares two levels.
    /// </para>
    /// </remarks>
    public static bool operator <(Level l, Level r)
    {
      return l.mLevelValue < r.mLevelValue;
    }

    /// <summary>
    /// Returns a value indicating whether a specified <see cref="Level" /> 
    /// is greater than or equal to another specified <see cref="Level" />.
    /// </summary>
    /// <param name="l">A <see cref="Level" /></param>
    /// <param name="r">A <see cref="Level" /></param>
    /// <returns>
    /// <c>true</c> if <paramref name="l" /> is greater than or equal to 
    /// <paramref name="r" />; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// <para>
    /// Compares two levels.
    /// </para>
    /// </remarks>
    public static bool operator >=(Level l, Level r)
    {
      return l.mLevelValue >= r.mLevelValue;
    }

    /// <summary>
    /// Returns a value indicating whether a specified <see cref="Level" /> 
    /// is less than or equal to another specified <see cref="Level" />.
    /// </summary>
    /// <param name="l">A <see cref="Level" /></param>
    /// <param name="r">A <see cref="Level" /></param>
    /// <returns>
    /// <c>true</c> if <paramref name="l" /> is less than or equal to 
    /// <paramref name="r" />; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// <para>
    /// Compares two levels.
    /// </para>
    /// </remarks>
    public static bool operator <=(Level l, Level r)
    {
      return l.mLevelValue <= r.mLevelValue;
    }

    /// <summary>
    /// Returns a value indicating whether two specified <see cref="Level" /> 
    /// objects have the same value.
    /// </summary>
    /// <param name="l">A <see cref="Level" /> or <see langword="null" />.</param>
    /// <param name="r">A <see cref="Level" /> or <see langword="null" />.</param>
    /// <returns>
    /// <c>true</c> if the value of <paramref name="l" /> is the same as the 
    /// value of <paramref name="r" />; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// <para>
    /// Compares two levels.
    /// </para>
    /// </remarks>
    public static bool operator ==(Level l, Level r)
    {
      if (((object)l) != null && ((object)r) != null)
      {
        return l.mLevelValue == r.mLevelValue;
      }
      else
      {
        return ((object)l) == ((object)r);
      }
    }

    /// <summary>
    /// Returns a value indicating whether two specified <see cref="Level" /> 
    /// objects have different values.
    /// </summary>
    /// <param name="l">A <see cref="Level" /> or <see langword="null" />.</param>
    /// <param name="r">A <see cref="Level" /> or <see langword="null" />.</param>
    /// <returns>
    /// <c>true</c> if the value of <paramref name="l" /> is different from
    /// the value of <paramref name="r" />; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// <para>
    /// Compares two levels.
    /// </para>
    /// </remarks>
    public static bool operator !=(Level l, Level r)
    {
      return !(l == r);
    }

    #endregion Operators

    #region Public Static Methods

    /// <summary>
    /// Compares two specified <see cref="Level"/> instances.
    /// </summary>
    /// <param name="l">The first <see cref="Level"/> to compare.</param>
    /// <param name="r">The second <see cref="Level"/> to compare.</param>
    /// <returns>
    /// A 32-bit signed integer that indicates the relative order of the 
    /// two values compared. The return value has these meanings:
    /// <list type="table">
    ///		<listheader>
    ///			<term>Value</term>
    ///			<description>Meaning</description>
    ///		</listheader>
    ///		<item>
    ///			<term>Less than zero</term>
    ///			<description><paramref name="l" /> is less than <paramref name="r" />.</description>
    ///		</item>
    ///		<item>
    ///			<term>Zero</term>
    ///			<description><paramref name="l" /> is equal to <paramref name="r" />.</description>
    ///		</item>
    ///		<item>
    ///			<term>Greater than zero</term>
    ///			<description><paramref name="l" /> is greater than <paramref name="r" />.</description>
    ///		</item>
    /// </list>
    /// </returns>
    /// <remarks>
    /// <para>
    /// Compares two levels.
    /// </para>
    /// </remarks>
    public static int Compare(Level l, Level r)
    {
      // Reference equals
      if ((object)l == (object)r)
      {
        return 0;
      }

      if (l == null && r == null)
      {
        return 0;
      }
      if (l == null)
      {
        return -1;
      }
      if (r == null)
      {
        return 1;
      }

      return l.mLevelValue.CompareTo(r.mLevelValue);
    }

    #endregion Public Static Methods

    #region Public Static Fields

    /// <summary>
    /// The <see cref="Off" /> level designates a higher level than all the rest.
    /// </summary>
    public readonly static Level Off = new Level(LevelValueOff, "OFF");

    /// <summary>
    /// The <see cref="Emergency" /> level designates very severe error events. 
    /// System unusable, emergencies.
    /// </summary>
    public readonly static Level Log4Net_Debug = new Level(LevelValueLog4Net_Debug, "Log4Net:DEBUG");

    /// <summary>
    /// The <see cref="Emergency" /> level designates very severe error events. 
    /// System unusable, emergencies.
    /// </summary>
    public readonly static Level Emergency = new Level(LevelValueEmergency, "EMERGENCY");

    /// <summary>
    /// The <see cref="Fatal" /> level designates very severe error events 
    /// that will presumably lead the application to abort.
    /// </summary>
    public readonly static Level Fatal = new Level(LevelValueFatal, "FATAL");

    /// <summary>
    /// The <see cref="Alert" /> level designates very severe error events. 
    /// Take immediate action, alerts.
    /// </summary>
    public readonly static Level Alert = new Level(LevelValueAlert, "ALERT");

    /// <summary>
    /// The <see cref="Critical" /> level designates very severe error events. 
    /// Critical condition, critical.
    /// </summary>
    public readonly static Level Critical = new Level(LevelValueCritical, "CRITICAL");

    /// <summary>
    /// The <see cref="Severe" /> level designates very severe error events.
    /// </summary>
    public readonly static Level Severe = new Level(LevelValueSevere, "SEVERE");

    /// <summary>
    /// The <see cref="Error" /> level designates error events that might 
    /// still allow the application to continue running.
    /// </summary>
    public readonly static Level Error = new Level(LevelValueError, "ERROR");

    /// <summary>
    /// The <see cref="Warn" /> level designates potentially harmful 
    /// situations.
    /// </summary>
    public readonly static Level Warn = new Level(LevelValueWarn, "WARN");

    /// <summary>
    /// The <see cref="Notice" /> level designates informational messages 
    /// that highlight the progress of the application at the highest level.
    /// </summary>
    public readonly static Level Notice = new Level(LevelValueNotice, "NOTICE");

    /// <summary>
    /// The <see cref="Info" /> level designates informational messages that 
    /// highlight the progress of the application at coarse-grained level.
    /// </summary>
    public readonly static Level Info = new Level(LevelValueInfo, "INFO");

    /// <summary>
    /// The <see cref="Debug" /> level designates fine-grained informational 
    /// events that are most useful to debug an application.
    /// </summary>
    public readonly static Level Debug = new Level(LevelValueDebug, "DEBUG");

    /// <summary>
    /// The <see cref="Fine" /> level designates fine-grained informational 
    /// events that are most useful to debug an application.
    /// </summary>
    public readonly static Level Fine = new Level(LevelValueFine, "FINE");

    /// <summary>
    /// The <see cref="Trace" /> level designates fine-grained informational 
    /// events that are most useful to debug an application.
    /// </summary>
    public readonly static Level Trace = new Level(LevelValueTrace, "TRACE");

    /// <summary>
    /// The <see cref="Finer" /> level designates fine-grained informational 
    /// events that are most useful to debug an application.
    /// </summary>
    public readonly static Level Finer = new Level(LevelValueFiner, "FINER");

    /// <summary>
    /// The <see cref="Verbose" /> level designates fine-grained informational 
    /// events that are most useful to debug an application.
    /// </summary>
    public readonly static Level Verbose = new Level(LevelValueVerbose, "VERBOSE");

    /// <summary>
    /// The <see cref="Finest" /> level designates fine-grained informational 
    /// events that are most useful to debug an application.
    /// </summary>
    public readonly static Level Finest = new Level(LevelValueFinest, "FINEST");

    /// <summary>
    /// The <see cref="All" /> level designates the lowest level possible.
    /// </summary>
    public readonly static Level All = new Level(LevelValueAll, "ALL");

    #endregion Public Static Fields
  }
}