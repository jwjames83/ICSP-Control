PROGRAM_NAME = 'ICSP-Test'

(***********************************************************)
(*          DEVICE NUMBER DEFINITIONS GO BELOW             *)
(***********************************************************)
DEFINE_DEVICE

// Master NX-1200
dvRS232_1			= 05001:001:000 // RS232-1 -> 
dvRS232_2			= 05001:002:000 // RS232-2 -> 
dvIRPort			= 05001:003:000 // Heartbeat
dvIOPort			= 05001:004:000 // -

// Touchpanel
dvPanel_01_01			= 10001:001:000	// Port 1
dvPanel_01_02			= 10001:002:000	// Port 2

dvIcsp_01_01			= 15000:001:000	// Port 1
dvIcsp_01_02			= 15000:002:000	// Port 2

vdMisc 				= 33000:001:000 // Misc

vdTest_01			= 33001:001:000 // Test
vdTest_02			= 33001:002:000 // Test

(***********************************************************)
(*               CONSTANT DEFINITIONS GO BELOW             *)
(***********************************************************)
DEFINE_CONSTANT

long nTimeLineHeartbeat       		= 1         	// Timeline für Heartbeat
long nTimeLineHeartbeatTime[] 		= { 1000 }  	// Interval: 1 Sekunde

// Touchpanel Arrays
dev dvTP_01[] = { dvPanel_01_01, dvIcsp_01_01 } // Port 1
dev dvTP_02[] = { dvPanel_01_02, dvIcsp_01_02 } // Port 2

(***********************************************************)
(*              DATA TYPE DEFINITIONS GO BELOW             *)
(***********************************************************)
DEFINE_TYPE

(***********************************************************)
(*               VARIABLE DEFINITIONS GO BELOW             *)
(***********************************************************)
DEFINE_VARIABLE

volatile integer nHeartbeat

volatile char     cStr[200]
volatile widechar wStr[200]

(***********************************************************)
(*       MUTUALLY EXCLUSIVE DEFINITIONS GO BELOW           *)
(***********************************************************)
DEFINE_MUTUALLY_EXCLUSIVE

(***********************************************************)
(*        SUBROUTINE/FUNCTION DEFINITIONS GO BELOW         *)
(***********************************************************)

DEFINE_MODULE

include 'DevStuff'

DEFINE_FUNCTION Log(char msg[])
    {
    send_string 0, msg
    }

define_call 'TpUpdate'
    {
    [dvTP_01, 1] = [dvIOPort, 1]
    [dvTP_01, 2] = [dvIOPort, 2]
    [dvTP_01, 3] = [dvIOPort, 3]
    [dvTP_01, 4] = [dvIOPort, 4]
    }

(***********************************************************)
(*                STARTUP CODE GOES BELOW                  *)
(***********************************************************)
DEFINE_START

Log("'=========================================================='")
Log("'System rebooted (', __name__, '), System: ', itoa(system_number)")
Log("'IP-Address: ', GetIpAddress(), ', Device: ', DevToStr(5001)")
Log("'File: ', __file__")
Log("'Last compiled: ', __ldate__, ' ', __time__")
Log("'=========================================================='")

send_string 0, 'Execute Startup Code: Main ...'

// Starten des Heartbeats
timeline_create(nTimeLineHeartbeat, nTimeLineHeartbeatTime, length_array(nTimeLineHeartbeatTime), timeline_relative, timeline_repeat)

(***********************************************************)
(*                THE EVENTS GO BELOW                      *)
(***********************************************************)
DEFINE_EVENT

// Timer Überwachung Heartbeat
timeline_event[nTimeLineHeartbeat]
    {
    nHeartbeat = !nHeartbeat
    
    // Heartbeat auf Controller
    [dvIRPort, 1] = nHeartbeat
    
    call 'TpUpdate'
    
    // Heartbeat Icon auf TP
    // [dvTP_01, BtnHeartbeat] = nHeartbeat
    }

data_event[dvTP_01]
    {
    online:
	{
	call 'TpUpdate'
	}
    }

data_event[dvRS232_1]
data_event[dvRS232_2]
data_event[dvIRPort]
data_event[dvIOPort]
data_event[dvPanel_01_01]
data_event[dvPanel_01_02]
data_event[dvIcsp_01_01]
data_event[dvIcsp_01_02]
data_event[vdMisc]
data_event[vdTest_01]
data_event[vdTest_02]
    {
    online:
	{
	send_string 0, "'Online-Event: Dev=', DevtoStr(Data.Device)"
	}
    offline:
	{
	send_string 0, "'Offline-Event: Dev=', DevtoStr(Data.Device)"
	}
    onerror:
	{
	send_string 0, "'Error-Event: Dev=', DevtoStr(Data.Device), ', Error=', itoa(Data.Number)"
	}
    string:
	{
	send_string 0, "'String-Event: Dev=', DevToStr(Data.Device), ', Data=', Data.Text"
	}
    command:
	{
	send_string 0, "'Command-Event: Dev=', DevToStr(Data.Device), ', Data=', Data.Text"
	}
    }

channel_event[dvRS232_1, 0]
channel_event[dvRS232_2, 0]
//channel_event[dvIRPort, 0]
channel_event[dvIOPort, 0]
//channel_event[dvPanel_01_01, 0]
//channel_event[dvPanel_01_02, 0]
//channel_event[dvIcsp_01_01, 0]
//channel_event[dvIcsp_01_02, 0]
//channel_event[vdMisc, 0]
channel_event[vdTest_01, 0]
channel_event[vdTest_02, 0]
channel_event[dvIOPort, 0]
    {
    on:
	{
	send_string 0, "'Channel-Event  On: Dev=', DevToStr(Channel.Device), ', Channel=', itoa(Channel.Channel)"
	}
    off:
	{
	send_string 0, "'Channel-Event Off: Dev=', DevToStr(Channel.Device), ', Channel=', itoa(Channel.Channel)"
	}
    }

channel_event[vdMisc, 0]
    {
    on:
	{
	send_string 0, "'Channel-Event  On: Dev=', DevToStr(Channel.Device), ', Channel=', itoa(Channel.Channel)"
	
	off[Channel.Device, Channel.Channel]
	
	switch(Channel.Channel)
	    {
	    case 1:
		{
		cStr = "'Testchar [€] -> Euro'"
		send_string dvIcsp_01_01, cStr
		}
	    case 2: 
		{
		cStr = "'Testchar [€] -> Euro'"
		send_command dvIcsp_01_01, cStr
		}
	    }
	}
    off:
	{
	send_string 0, "'Channel-Event Off: Dev=', DevToStr(Channel.Device), ', Channel=', itoa(Channel.Channel)"
	}
    }

level_event[dvRS232_1, 0]
level_event[dvRS232_2, 0]
level_event[dvIRPort, 0]
level_event[dvIOPort, 0]
level_event[dvPanel_01_01, 0]
level_event[dvPanel_01_02, 0]
level_event[dvIcsp_01_01, 0]
level_event[dvIcsp_01_02, 0]
level_event[vdMisc, 0]
level_event[vdTest_01, 0]
level_event[vdTest_02, 0]
level_event[dvIOPort, 0]
level_event[vdMisc, 0]
    {
    send_string 0, "'Level-Event: Dev=', DevToStr(Level.Input.Device), ', Level=', itoa(Level.Input.Level), ', Value=', itoa(Level.Value)"
    }

data_event[dvIRPort]
    {
    online:
	{
	// NX-1200
	send_command dvIRPort, 'Set Mode Serial'
	}
    }

(***********************************************************)
(*            THE ACTUAL PROGRAM GOES BELOW                *)
(***********************************************************)
DEFINE_PROGRAM

(***********************************************************)
(*                     END OF PROGRAM                      *)
(*        DO NOT PUT ANY CODE BELOW THIS COMMENT           *)
(***********************************************************)