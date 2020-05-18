PROGRAM_NAME = 'DevStuff'

(***********************************************************)
(*          DEVICE NUMBER DEFINITIONS GO BELOW             *)
(***********************************************************)
DEFINE_DEVICE

(***********************************************************)
(*               CONSTANT DEFINITIONS GO BELOW             *)
(***********************************************************)
DEFINE_CONSTANT

integer MaxStringSize	= 1024	// Maximum string return size
integer MaxIoOutMtu	= 131	// Max characters to transmit (the
				// NL diagnositics console will only
				// display a max of 131 characters per line)
char    IoLineSeperator	= $0A	// Seperator to insert between lines

(***********************************************************)
(*              DATA TYPE DEFINITIONS GO BELOW             *)
(***********************************************************)
DEFINE_TYPE

(***********************************************************)
(*               VARIABLE DEFINITIONS GO BELOW             *)
(***********************************************************)
DEFINE_VARIABLE

dev dvIoOut = 0:0:0 // Output stream device

(***********************************************************)
(*       MUTUALLY EXCLUSIVE DEFINITIONS GO BELOW           *)
(***********************************************************)
DEFINE_MUTUALLY_EXCLUSIVE

(***********************************************************)
(*        SUBROUTINE/FUNCTION DEFINITIONS GO BELOW         *)
(***********************************************************)

define_function char[15] GetIpAddress()
    {
    stack_var ip_address_struct IP

    get_ip_address(0:0:0, IP)

    return IP.IPAddress
    }

define_function char[15] GetDevIpAddress(dev device)
    {
    stack_var ip_address_struct IP

    get_ip_address(device, IP)

    return IP.IPAddress
    }

define_function char[15] DevToStr(dev device)
    {
    return "itoa(device.Number), ':', itoa(device.Port), ':', itoa(device.System)"
    }

define_function sinteger Equals(char str1[], char str2[])
    {
    return compare_string(upper_string(str1), upper_string(str2))
    }

define_function char[1024] IntToStr(integer value, integer length)
    {
    stack_var char    cResult[1024]
    stack_var integer nIndex

    cResult = itoa(value)

    if (length > 1024 || length <= 0)
	return cResult

    while(length_string(cResult) < length)
	cResult = "'0', cResult"

    return cResult
    }

define_function char[2] IntToHex(integer value)
    {
    return right_string("'0', itohex(value)", 2)
    }

define_function char[5] IntToBool(integer value)
    {
    if(value)
	return 'true'
    else 
	return 'false'
    }

define_function ThreadWait(integer ms)
    {
    set_timer(0)

    while(true)
	{
	if(get_timer >= ms)
	    break
	}
    }

// =====================================================================================================
// String-Funktionen
// =====================================================================================================

/*
 * Write a character array to the output device.
 *
 * If the outgoing data exceeds MaxIoOutMtu it will be split into multiple
 * packets. If a non printable char is located within the last half of the
 * current packet it will split at the non printable character, otherwise all
 * data up to MaxIoOutMtu will be sent in the packet.
 *
 * If MaxIoOutMtu is 0 all data will be sent in a single packet.
 *
 * @param buf   : The character array to write
 * @param offset: The offset to begin writing from (offset 0 == character 1)
 * @param len   : The number of characters to write
 */
define_function WriteString(char buffer[], integer offset, integer len)
    {
    stack_var integer nStart
    stack_var integer nEnd
    stack_var integer nMinLen

    if (MaxIoOutMtu && len > MaxIoOutMtu)
	{
	nMinLen = type_cast(MaxIoOutMtu / 2)

	nStart = offset + 1

	while (nStart < offset + len)
	    {
	    nEnd = min_value(nStart + MaxIoOutMtu, offset + len)

	    while (nEnd > nStart + nMinLen)
		{
		if ((buffer[nEnd] > $08 && buffer[nEnd] < $0E) || (buffer[nEnd] > $1B && buffer[nEnd] < $21))
		    {
		    nEnd++
		    break
		    }
		nEnd--
		}

	    if (nEnd <= nStart + nMinLen)
		{
		nEnd = min_value(nStart + MaxIoOutMtu, offset + len + 1)
		}

	    WriteString(buffer, nStart - 1, nEnd - nStart)

	    nStart = nEnd
	    }
	}
    else
	{
	send_string dvIoOut, mid_string(buffer, offset + 1, len)
	}
    }

/*
 * Schreibt die angegebene Zeichenfolge in den Standardausgabestream.
 *
 * @param value: Der zu schreibende Wert.
 */
define_function Write(char value[])
    {
    WriteString(value, 0, length_string(value))
    }

/*
 * Schreibt die angegebene Zeichenfolge, gefolgt vom aktuellen Zeichen
 * für den Zeilenabschluss, in den Standardausgabestream.
 *
 * @param	value: Der zu schreibende Wert.
 */
define_function WriteLine(char value[])
    {
    WriteString("value, IoLineSeperator", 0, length_string(value))
    }

/**
 * Checks to see if the passed character is a whitespace character.
 *
 * @param value: the character to check
 * @return a boolean value specifying whether the character is whitespace
 */
define_function char IsWhitespace(char value)
    {
    return (value >= $09 && value <= $0D) || (value >= $1C && value <= $20);
    }

/**
 * Replace all occurances of a substring with another string.
 *
 * @param value   : The string to search
 * @param oldValue: The substring to replace
 * @param newValue: The replacement subtring
 * @return: 'value' with all occurances of 'search' replaced by the contents of 'newValue'
 */
define_function char[MaxStringSize] ReplaceStr(char value[], char oldValue[], char newValue[])
    {
    stack_var integer nStart
    stack_var integer nEnd
    stack_var char    cStr[MaxStringSize]

    nStart = 1
    nEnd = find_string(value, oldValue, nStart)

    while(nEnd)
	{
	cStr   = "cStr, mid_string(value, nStart, nEnd - nStart), newValue"
	nStart = nEnd + length_string(oldValue)
	nEnd   = find_string(value, oldValue, nStart)
	}

    cStr = "cStr, right_string(value, length_string(value) - nStart + 1)"

    return cStr;
    }

// TT.MM.JJJJ
define_function char[10] DateToStr()
    {
    stack_var integer nDay
    stack_var integer nMonth
    stack_var integer nYear

    nDay   = type_cast(date_to_day(ldate))
    nMonth = type_cast(date_to_month(ldate))
    nYear  = type_cast(date_to_year(ldate))

    return "IntToStr(nDay, 2), '.', IntToStr(nMonth, 2), '.', itoa(nYear)";
    }

// HH:MM:SS
define_function char[8] TimeToStr()
    {
    // Time:
    // This system variable holds the current time as a string in the form "hh:mm:ss".
    // The time is represented in 24-hour format.

    return time;
    }

/**
 * Returns a copy of the string with the left whitespace removed. If no
 * printable characters are found, an empty string will be returned.
 *
 * @param value: A string to trim
 * @return: The original string with left whitespace removed
 */
define_function char[MaxStringSize] LTrim(char value[])
    {
    stack_var char    cStr[MaxStringSize + 1]
    stack_var integer nIndex
    stack_var integer nLength

    nLength = length_string(value)

    for (nIndex = 1; nIndex <= nLength; nIndex++)
	{
	if (!IsWhitespace(value[nIndex]))
	    {
	    cStr = right_string(value, nLength - nIndex + 1)
	    return cStr;
	    }
	}
    }

/**
 * Returns a copy of the string with the right whitespace removed. If no
 * printable characters are found, an empty string will be returned.
 *
 * @param value: A string to trim
 * @return: The string with right whitespace removed
 */
define_function char[MaxStringSize] RTrim(char value[])
    {
    stack_var integer nIndex
    stack_var integer nLength

    nLength = length_string(value)

    for (nIndex = nLength; nIndex; nIndex--)
	{
	if(!IsWhitespace(value[nIndex]))
	    return left_string(value, nIndex);
	}
    }

/**
 * Returns a copy of the string with the whitespace removed. If no printable
 * characters are found, an empty string will be returned.
 *
 * @param value: A string to trim
 * @return: The original string with whitespace removed
 */
define_function char[MaxStringSize] Trim(char value[])
    {
    return LTrim(RTrim(value));
    }
(***********************************************************)
(*                STARTUP CODE GOES BELOW                  *)
(***********************************************************)
DEFINE_START

(***********************************************************)
(*                THE EVENTS GO BELOW                      *)
(***********************************************************)
DEFINE_EVENT

(***********************************************************)
(*            THE ACTUAL PROGRAM GOES BELOW                *)
(***********************************************************)
DEFINE_PROGRAM

(***********************************************************)
(*                     END OF PROGRAM                      *)
(*        DO NOT PUT ANY CODE BELOW THIS COMMENT           *)
(***********************************************************)