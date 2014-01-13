;************************************************************************
;          MICROCOSM ASSOCIATES DISK CONTROLLER DIAGNOSTIC
;                              FOR
;                   ICOM FD3712  VER.1.0 (C) 1979
;************************************************************************
;
;DONATED TO THE "SIG/M" CP/M USER'S GROUP BY:
;
;KELLY SMITH OF MICROCOSM ASSOCIATES
;3055 WACO AVENUE
;SIMI VALLEY, CALIFORNIA
;(805) 527-9321 (MODEM, CP/M-NET (TM))
;(805) 527-0518 (VERBAL)
;
;
	ORG	 00100H	;ORG FOR CP/M
;
;
;
         DI                      ;DISABLE INTERRUPTS
         LXI     SP,STACK        ;SET THE STACK POINTER
         JMP     INIT            ;INITIALIZE
;
;
;DEFINE CONSOLE I/O PARAMETERS FOR ALTAIR 2SI/O BOARD
;
CCTRL    EQU     010H    ;CONSOLE COMMAND/STATUS PORT
CDATA    EQU     011H    ;CONSOLE DATA PORT
CRRDY    EQU     001H    ;RECEIVER READY BIT
CTRDY    EQU     002H    ;TRANSMITTER READY BIT
;
;
;
;DEFINE SINGLE DENSITY CONTROLLER I/O PARAMETERS
;
CNTRL    EQU     0C0H    ;CONTROL COMMANDS
DATAO    EQU     0C1H    ;DATA OUT
DATAI    EQU     0C0H    ;DATA IN
;
;
;
;*************************
;* MONITOR CODE FOLLOWS: *
;*************************
;
;

;SUBROUTINE TO CONVERT TWO HEX CHARACTERS TO ONE BYTE

NBL:     SUI     '0'
         RC
         ADI     0E9H
         RC
         ADI     6
         JP      NIO
         ADI     7
         RC
NIO:     ADI     10
         ORA     A
         RET
;
;
;
;SUBROUTINE TO CONVERT A BYTE TO TWO HEX CHARACTERS

LBYTE:   PUSH    PSW
         RRC
         RRC
         RRC
         RRC
         ANI     0FH
         CALL    HXDA
         POP     PSW
         ANI     0FH
         JMP     HXDA

HXDA:    CPI     10
         JM      CNO
         ADI     7
CNO:     ADI     30H
         MOV     C,A
         JMP     CO

HXD      EQU     LBYTE
;
;        CONSOLE INPUT ROUTINE
;
CI:      IN      CCTRL
         ANI     CRRDY
         JZ      CI
         IN      CDATA
         ANI     7FH
         RET
;
;
;
CRLF:    MVI     C,0DH
         CALL    CO
         MVI     C,0AH
         JMP     CO
;
CONO:    CALL    BYTEO
         CALL    CRLF
         RET
;
CONI:    CALL    BYTEC
         PUSH    PSW
         POP     PSW
         RET
;
;        CONSOLE OUTPUT ROUTINE
;
CO:      IN      CCTRL
         ANI     CTRDY
         JZ      CO
         MOV     A,C
         OUT     CDATA
         RET
;
;
;
;
;
;INITIALIZE ALL PROGRAM CONTROL PARAMETERS AND OUTPUT OPERATING INSTRUCTIONS
;
INIT:    MVI     A,003H  ;SET-UP CONSOLE SERIAL I/O
         OUT     CCTRL
         MVI     A,0B5H
         OUT     CCTRL
         CALL    TEXT    ;OUTPUT OPERATING INSTRUCTIONS TO CONSOLE
;
;
;
;TEXT TABLE OF OPERATING INSTRUCTIONS
;
	 DB	 0DH,0AH,'MICROCOSM DISK DIAGNOSTIC FOR ICOM FD3712'
	 DB	 0DH,0AH,0AH,'COPYRIGHT 1979 MICROCOSM ASSOCIATES'
         DB      0DH,0AH,0AH,'OPERATING INSTRUCTION ARE AS FOLLOWS:'
         DB      0DH,0AH
         DB      0DH,0AH,20H,'RESPOND TO ALL "=" CHARACTERS, '
         DB      'WITH A TWO DIGIT HEX VALUE OR '
         DB      'ALPHA CHARACTER '
         DB      0DH,0AH
         DB      'FOR ALL TEST FUNCTIONS EXCEPT,'
         DB      0DH,0AH,'MEMORY ADDRESS(0000-FFFF)=,'
         DB      'AND GO ADDRESS(0000-FFFF)='
         DB      0DH,0AH,'(FOUR DIGIT HEX VALUES ARE REQUIRED'
         DB      ',TERMINATED WITH CARRIAGE-RETURN)'
         DB      0DH,0AH,0AH,020H
         DB      'IMPROPER KEY-BOARD ENTRYS WILL BE '
         DB      'PROMPTED,WITH A "?" CHARACTER,'
         DB      0DH,0AH,'OR AN "INVALID" MESSAGE '
         DB      ' (PLEASE PRESS SPACE-BAR)'
;
;
;
         DB      080H    ;END OF TEXT FLAG
;
;
;
         MVI     B,11    ;SET-UP 11 LINE-FEEDS
         CALL    LFEED
         CALL    BAR     ;WAIT FOR OPERATOR TO PRESS SPACE-BAR
;
;
;
         CALL    TEXT    ;OUTPUT MORE TEXT TO CONSOLE
;
;
;
         DB      0DH,0AH,0AH,0AH,0AH,20H
         DB      'THE FOLLOWING IS A '
         DB      'LIST OF ALL COMMANDS:'
         DB      0DH,0AH,20H
         DB      'ENTER A KEY-BOARD CHARACTER '
         DB      'AS REQUIRED,FOR '
         DB      'TEST EXECUTION'
         DB      0DH,0AH
         DB      0DH,0AH,20H,'M.....MEMORY ADDRESS(0000-FFFF)='
         DB      0DH,0AH,20H,'G.....GO ADDRESS(0000-FFFF)='
         DB      0DH,0AH,20H,'U.....UNIT SELECT(00-03)='
         DB      0DH,0AH,20H,'L.....LOOP'
         DB      0DH,0AH,20H,'CONTROL-A.....ALIGNMENT,AUTO(Y,N)?'
         DB      0DH,0AH,20H,'H.....HOME'
         DB      0DH,0AH,20H,'C.....CLEAR'
         DB      0DH,0AH,20H,'CONTROL-T.....TIMER CONTROL(Y,N)?'
         DB      0DH,0AH,20H,'CONTROL-P.....WRITE PROTECT(Y,N)?'
         DB      0DH,0AH,20H,'S.....SEEK TO TRACK(00-4C)='
         DB      0DH,0AH,20H,'R.....READ SECTOR(01-1A)='
         DB      0DH,0AH,20H,'W.....WRITE SECTOR(01=1A)='
         DB      0DH,0AH,20H,'P.....PATTERN(00-FF)='
         DB      0DH,0AH,20H,'T.....TEST WRITE/READ CONTINUOUS'
         DB      0DH,0AH,20H,'CONTROL-S.....SYSTEM TEST'
         DB      0DH,0AH,07H,20H
         DB      'SPACE-BAR.....ABORTS ANY '
         DB      'TEST OR "HANG-UP" '
         DB      '  (PLEASE PRESS SPACE-BAR)'
;
;
;
         DB      080H    ;END OF TEXT FLAG
;
;
;
         CALL    BAR     ;WAIT FOR OPERATOR TO PRESS SPACE-BAR
         MVI     B,25    ;SET-UP 25 LINE-FEEDS
         CALL    LFEED
;
;
;
         CALL    TEXT    ;OUTPUT MORE TEXT TO CONSOLE
;
;
;
         DB      0DH,0AH,20H
         DB      'THE FOLLOWING IS A SYSTEM '
         DB      'STATUS TEST:'
         DB      0DH,0AH
;
;
;
         DB      080H    ;END OF TEXT FLAG
;
;
;
         MVI     A,001H  ;SET SECTOR NUMBER TO 01
         STA     SECNO
         XRA     A       ;CLEAR ALL UNITS OF DISK REMOVED STATUS,
                         ; AND INDICATE ANY UNITS WITH 
                         ; DISK FAIL STATUS
         CALL    TCHK    ;TEST CHECK,UNIT 00
         MVI     A,040H
         CALL    TCHK    ;TEST CHECK,UNIT 01
         MVI     A,080H
         CALL    TCHK    ;TEST CHECK,UNIT 02
         MVI     A,0C0H
         CALL    TCHK    ;TEST CHECK,UNIT 03
         MVI     B,10    ;OUTPUT 10 LINE-FEEDS TO CONSOLE
         CALL    LFEED
         XRA     A
         STA     UNIT    ;SET UP INITIAL SELECT FOR DRIVE 00
         STA     DELET   ;CLEAR DELETED DATA MARK COMMAND
         STA     RETRY   ;CLEAR READ ERROR RETRY COUNTER
         STA     TIME    ;CLEAR TIMER COMMAND
         STA     SAVE    ;CLEAR "SAVED STATUS"
         STA     RDCRC   ;CLEAR READ CRC COMMAND
         STA     WPROT   ;CLEAR WRITE PROTECT COMMAND
         STA     VFYCM   ;CLEAR VERIFY COMMAND
         LXI     H,000H
         SHLD    ECTR    ;CLEAR ERROR COUNTER
         SHLD    PCTR    ;CLEAR PASS COUNTER
         MVI     A,005H  ;SET NUMBER OF ERROR RETRIES TO 5 DECIMAL
         STA     TRIES
;
;
;
MNTR:    XRA     A       ;CLEAR A REG.
         STA     BLOOP   ;CLEAR LOOP COMMAND
         STA     VFYCM   ;CLEAR VERIFY COMMAND
MNTR1:   LXI     SP,STACK
         CALL    CRLF
         MVI     C,07H   ;OUTPUT BELL
         CALL    CO
         MVI     C,40H   ;OUTPUT PROMPT (@ CHARACTER)
         CALL    CO
         CALL    CECHO   ;ECHO KEY-BOARD INPUT OUT TO CONSOLE
         CPI     'M'
         JZ      MEM     ;JUMP TO MEMORY DISPLAY/ALTER
         CPI     'G'
         JZ      GO      ;JUMP TO GO
         CPI     'U'
         JZ      UNITS   ;JUMP TO UNIT SELECT
         CPI     'H'
         JZ      HOME    ;JUMP TO HOME (SEEK TO TRACK 00)
         CPI     'S'
         JZ      SKT     ;JUMP TO SEEK TO TRACK
         CPI     013H
         JZ      SYS     ;JUMP TO SYSTEM TEST,IF "CONTROL-S"
         CPI     'W'
         JZ      WRT     ;JUMP TO WRITE SECTOR
         CPI     'R'
         JZ      RDT     ;JUMP TO READ SECTOR
         CPI     'C'
         JZ      CLR     ;JUMP TO CLEAR CONTROLLER
         CPI     'T'
         JZ      RWC     ;JUMP TO WRITE/READ CONTINUOUS
         CPI     'P'
         JZ      PATT    ;JUMP TO SET DATA PATTERN
         CPI     'L'
         JZ      LOOP    ;JUMP TO SET LOOP COMMAND
         CPI     014H    
         JZ      TIMER   ;SET TIMER CONTROL,IF "CONTROL-T"
         CPI     010H
         JZ      PROT    ;SET WRITE PROTECT,IF "CONTROL-P"
         CPI     001H
         JZ      ALIN    ;JUMP TO ALIGNMENT,IF "CONTROL-A"
         CPI     'V'
         JZ      VRFYT   ;JUMP TO VERIFY (READ CRC,ALL TRACKS/SECTORS)
LER:     MVI     C,'?'   ;LINE INPUT ERROR, OUTPUT "?" TO CONSOLE
         CALL    CO
         JMP     MNTR
;
;
CECHO:   CALL    CI
         MOV     C,A
         CALL    CO
         RET
;
;
GO:      LXI     H,MSG25         ;OUTPUT GO ADDRESS? TO CONSOLE
         CALL    MSG
         CALL    PARAM
         CALL    CRLF
         PCHL
;
;
PARAM:   LXI     H,0
PARM1:   CALL    CECHO
         CPI     0DH
         RZ
         DAD     H
         DAD     H
         DAD     H
         DAD     H
         JC      LER
         CALL    NBL
         JC      LER
         ORA     L
         MOV     L,A
         JMP     PARM1
;
;
BYTEC:   CALL    CECHO
BYTC1:   CALL    NBL
         JC      LER
         RLC
         RLC
         RLC
         RLC
         PUSH    PSW
         CALL    CECHO
         CALL    NBL
         JC      LER
         POP     B
         ORA     B
         RET
;
;
BYTEO:   PUSH    PSW
         CALL    BYTO1
         MOV     C,A
         CALL    CO
         POP     PSW
         CALL    BYTO2
         MOV     C,A
         JMP     CO
;
;
BYTO1:   RRC
         RRC
         RRC
         RRC
BYTO2:   ANI     0FH
         CPI     0AH
         JM      BYTO3
         ADI     7
BYTO3:   ADI     30H
         RET
;
;
HLCO:    CALL    CRLF
         MOV     A,H
         CALL    BYTEO
         MOV     A,L
         CALL    BYTEO
         RET
;
;
DSPYM:   CALL    HLCO
         MVI     C,'='
         CALL    CO
         MOV     A,M
         CALL    BYTEO
         MVI     C,20H
         CALL    CO
         RET
;
;
MEM:     LXI     H,MSG14
         CALL    MSG
         CALL    PARAM
MEM1:    CALL    DSPYM
         CALL    CECHO
         CPI     05EH    ;NEED TO BACK-UP?
         JZ      MEM10
         CPI     0DH
         JZ      MNTR
         CPI     20H
         JZ      MEM9
         CALL    BYTC1
         MOV     M,A
MEM9:    INX     H
         JMP     MEM1
MEM10:   DCX     H
         JMP     MEM1
;
;
;
TIMER:   LXI     H,MSG45         ;OUTPUT "TIMER CONTROL?" TO CONSOLE
         CALL    MSG
         CALL    CECHO
         CPI     'Y'
         JZ      TIMEY
         CPI     'N'
         JZ      TIMEN
         JMP     LER
TIMEY:   MVI     A,001H  ;SET TIMER CONTROL
         JMP     TIMES
TIMEN:   XRA     A       ;RESET TIMER CONTROL
TIMES:   STA     TIME
         JMP     MNTR1
;
;
;
;SUBROUTINE TO KILL TIME AS REQUIRED
;(TOTAL DELAY OF TIME1+TIME2+TIME3 EQUALS 2.5 SECONDS)
;
TIME1:   LXI     H,0FFFFH        ;LOAD DELAY CONSTANT
TIMEA:   DCX     H       ;-1 TO H&L REGS.
         MOV     A,L
         ORA     H
         JNZ     TIMEA   ;GO AGAIN,UNTIL TIMED OUT 
TIME2:   LXI     H,0FFFFH
TIMEB:   DCX     H
         MOV     A,L
         ORA     H
         JNZ     TIMEB
TIME3:   LXI     H,00FFFH
TIMEC:   DCX     H
         MOV     A,L
         ORA     H
         JNZ     TIMEC
         RET
;
;
;
;KEY-BOARD INTERRUPT ROUTINE, KEYED WITH "SPACE-BAR"
;
KBINT:   IN      CCTRL
         ANI     CRRDY
         RZ
         IN      CDATA
         ANI     07FH    ;MASK-OFF PARITY BIT 
         CPI     020H
         JZ      MNTR    ;GO TO MONITOR IF SPACE-BAR
         RET
;
;
;
;ROUTINE TO OUTPUT LINE-FEEDS TO CONSOLE
; (ASSUMES QUANTITY OF LINE-FEEDS TO BE IN B REG.)
;
LFEED:   MVI     C,00AH  ;OUTPUT A LINE-FEED TO CONSOLE
         CALL    CO
         DCR     B
         JNZ     LFEED
         RET
;
;
;
;ROUTINE TO "PAUSE" WHILE OPERATOR READS TEXT
; (ASSUMES PAUSE ITERATIONS TO BE IN B REG.)
;
PAUSE:   LXI     H,0FFFFH        ;SET-UP PAUSE DELAY
         CALL    DLY
         DCR     B
         JNZ     PAUSE
         RET
;
;
;
;ROUTINE TO DO TEST CHECK ON DRIVES
; (ASSUMES UNIT NUMBER TO BE IN A REG.)
;
TCHK:    STA     UNIT    ;SAVE UNIT NUMBER
         CALL    XMTUS
         CALL    UNCHK   ;CHECK UNIT CODE BITS FOR A MATCH
                         ; TO THE SELECTED UNIT
         CALL    WPCHK   ;CHECK WRITE PROTECT STATUS
         IN      DATAI   ;GET STATUS
         ANI     020H    ;DRIVE FAIL?
         RNZ
         CALL    CRLF
         CALL    SP1
         LXI     H,MSG31         ;OUTPUT "UNIT=" TO CONSOLE
         CALL    MSG
         CALL    AUNIT   ;OUTPUT UNIT NUMBER TO CONSOLE
         CALL    BYTEO
         CALL    SP1
         LXI     H,MSG24         ;OUTPUT "READY FOR TEST" TO CONSOLE
         CALL    MSG
         CALL    CRLF    ;KEEP IT NEAT 
         RET
;
;
;
;ROUTINE TO WAIT FOR SPACE-BAR FROM KEY-BOARD
;
BAR:     IN      CCTRL
         ANI     CRRDY
         JZ      BAR     ;OOPS KEY-BOARD NOT ACTIVE   
         IN      CDATA
         ANI     07FH    ;MASK-OFF PARITY BIT
         CPI     020H    ;SPACE-BAR?
         JNZ     BAR     ;NO KEEP WAITING 
         RET
;
;
;
;ROUTINE TO CAUSE "LOOPING" ON A TEST
;
LOOP:    LXI     H,MSG38         ;OUTPUT "LOOP" TO CONSOLE
         CALL    MSG
         MVI     A,001H  ;STORE LOOP COMMAND
         STA     BLOOP
         JMP     MNTR1
;
;
;
;SUBROUTINE TO TEST LOOP COMMAND
;
LOOPT:   CALL    KBINT   ;TEST KEY-BOARD INTERRUPT
         LDA     BLOOP   ;GO GET LOOP COMMAND
         ADI     000H    ;IS IT A LOOP COMMAND?
         RNZ             ;RETURN,IF YES 
         JMP     MNTR
;
;
;
;
;
;
;*****************************
;*DISK TEST CODE FOLLOWS:    *
;*****************************
;
;
;
;
SP5:     MVI     C,20H
         CALL    CO
SP4:     MVI     C,20H
         CALL    CO
SP3:     MVI     C,20H
         CALL    CO
SP2:     MVI     C,20H
         CALL    CO
SP1:     MVI     C,20H
         CALL    CO
         RET
;
;
;
;
;
YNMSG:   DB      5,'(Y,N)'
;
MSG1:    DB      8,0DH,0AH,' PASS='
;
MSG2:    DB      6,' WRITE'
;
MSG3:    DB      5,' READ'
;
MSG4:    DB      23,0DH,0AH,'INVALID TRACK NUMBER '
;
MSG5:    DB      23,0DH,0AH,' DATA COMPARE ERROR: '
;
;MSG6 RESERVED
;
;MSG7 RESERVED
;
;MSG8 RESERVED
;
;MSG9 RESERVED
;
MSG10:   DB      14,'UNIT CODE FAIL'
;
MSG11:   DB      20,'EEK TO TRACK(00-4C)='
;
MSG12:   DB      14,'SECTOR(01-1A)='
;
MSG13:   DB      14,'ATTERN(00-FF)='
;
MSG14:   DB      25,'EMORY ADDRESS(0000-FFFF)='
;
MSG15:   DB      33,0DH,0AH,' UNIT TRACK SECTOR BYTE WAS S/B'
;
MSG16:   DB      14,' TOTAL ERRORS='
;
MSG17:   DB      15,0DH,0AH,' MEDIA ERROR '
;
;MSG18 RESERVED
;
;MSG19 RESERVED
;
;MSG20 RESERVED
;
;MSG21 RESERVED
;
;MSG22 RESERVED
;
MSG23:   DB      12,0DH,0AH,20H,'FAILED TO'
;
MSG24:   DB      14,'READY FOR TEST'
;
MSG25:   DB      21,'O ADDRESS(0000-FFFF)='
;
MSG26:   DB      3,'OME'
;
MSG27:   DB      5,'RITE '
;
MSG28:   DB      4,'EAD '
;
MSG29:   DB      4,'LEAR'
;
MSG30:   DB      25,'EST WRITE/READ CONTINUOUS'
;
MSG31:   DB      5,'UNIT='
;
MSG32:   DB      18,'NIT SELECT(00-03)='
;
MSG33:   DB      22,0DH,0AH,'INVALID UNIT NUMBER '
;
MSG34:   DB      10,'DRIVE FAIL'
;
MSG35:   DB      13,'WRITE PROTECT'
;
;MSG36 RESERVED
;
MSG37:   DB      22,'INVALID SECTOR NUMBER '
;
MSG38:   DB      3,'OOP'
;
MSG39:   DB      14,0DH,0AH,07H,' CRC ERROR '
;
;MSG40 RESERVED
;
;MSG41 RESERVED
;
;MSG42 RESERVED
;
MSG43:   DB      19,' DELETE DATA(Y,N)? '
;
MSG44:   DB      8,' DD MARK'
;
MSG45:   DB      20,'TIMER CONTROL(Y,N)? '

;
MSG46:   DB      16,' CRC ONLY(Y,N)? '
;
;MSG47 RESERVED
;
MSG48:   DB      9,0DH,0AH,20H,'VERIFY'
;
MSG49:   DB      25,'ERIFY SINGLE TRACK(Y,N)? '
;
;MSG50 RESERVED
;
MSG51:   DB      10,'ALIGNMENT,'
;
MSG52:   DB      2,'NO'
;
MSG53:   DB      11,'AUTO(Y,N)? '
;
MSG54:   DB      13,'TRACK(00-4C)='
;
MSG55:   DB      22,0DH,0AH,20H,'NO "BUSY" RESPONSE '
;
MSG56:   DB      19,0DH,0AH,20H,'"BUSY" RUN-AWAY '
;
MSG57:   DB      11,'SYSTEM TEST'
;
MSG58:   DB      6,'TRACK='
;
MSG59:   DB      7,'SECTOR='
;
MSG60:   DB      14,'SEEK TO TRACK='
;
;
;
;
;
;ROUTINE TO OUTPUT MESSAGES TO CONSOLE
;
MSG:     PUSH    PSW
         PUSH    B
         MOV     B,M
         INX     H
MSGA:    MOV     C,M
         CALL    CO
         INX     H
         DCR     B
         JNZ     MSGA
         POP     B
         POP     PSW
         RET
;
;
;
;ROUTINE TO OUTPUT "TEXT" TO CONSOLE
;
TEXT:    XTHL            ;GET ADDRESS OF TEXT
         PUSH    B
         PUSH    PSW
TEXTA:   MOV     A,M     ;CHECK FOR "END OF TEXT" FLAG
         INX     H
         ORA     A
         JM      TEXTB   ;END OF TEXT?
         MOV     C,A
         CALL    CO
         JMP     TEXTA   ;NO TRY AGAIN   
TEXTB:   POP     PSW
         POP     B
         XTHL            ;GET THE RETURN ADDRESS
         RET
;
;
;
;*****************************************
;* CONTROLLER/DISK TEST ROUTINES FOLLOW: *
;*****************************************
;
;
;
;CLEAR CONTROLLER



CLR:     LXI     H,MSG29         ;OUTPUT "CLEAR" TO CONSOLE
         CALL    MSG
CLR1:    MVI     A,81H   ;TRANSMIT CLEAR COMMAND TO CONSOLE
         CALL    XMIT
         MVI     A,00BH  ;TRANSMIT CLEAR ERROR FLAGS TO CONTROLLER
         CALL    XMIT
         LXI     H,000H  ;CLEAR PASS AND ERROR COUNTER
         SHLD    PCTR
         SHLD    ECTR
         CALL    LOOPT   ;GO TEST LOOP COMMAND
         JMP     CLR1
;
;
;
;RESTORE TO TRACK 00
;
HOME:    LXI     H,MSG26         ;OUTPUT "HOME" TO CONSOLE

         CALL    MSG
HOME1:   CALL    RSTR    ;TRANSMIT "SEEK TRACK 00" COMMAND,
                         ; TO CONTROLLER
         CALL    LOOPT   ;GO TEST LOOP COMMAND
         JMP     HOME1
;
;
;

;SEEK TO TRACK

SKT:     LXI     H,MSG11         ;OUTPUT "SEEK TO TRACK=" TO CONSOLE
         CALL    MSG
         CALL    CONI    ;READ CONSOLE INPUT
         STA     TRKNO   ;STORE TRACK ADDRESS
         STA     NEWTK   ;STORE NEW TRACK NUMBER IN CASE OF LOOP 
         CPI     04DH    ;TRACK NUMBER >77 DECIMAL?
         JNC     INER1   ;JUMP TO INPUT ERROR 1,IF SO 
         CALL    CRLF
SKT1:    MVI     C,'.'   ;MAKE A TRACK SEEK INDICATOR FOR CONSOLE
         CALL    CO
         LXI     D,128
                         ; FOR SEEKING
         LDA     TRKNO   ;GO GET THE TRACK NUMBER
         CPI     000H    ;TRACK 00 REQUESTED?
         JZ      HOME1   ;IF SO,GO DO A RESTORE 
         CALL    CLRER   ;TRANSMIT "CLEAR ERROR FLAGS",
                         ; TO CONTROLLER
         CALL    XMTUS
         CALL    XMTTK
         MVI     A,009H  ;SEEK TO TRACK
         CALL    XMITW
         IN      DATAI   ;EXAMINE STATUS FROM CONTROLLER
         ANI     008H    ;CRC ERROR?
         JZ      SKT2    ;IF NOT,GO TEST LOOP COMMAND
         LXI     H,MSG39         ;OUTPUT "CRC ERROR" TO CONSOLE
         CALL    MSG
         CALL    CLRER   ;GO CLEAR ERROR FLAGS 
SKT2:    CALL    LOOPT   ;GO TEST LOOP COMMAND
         CALL    RSTR    ;TRANSMIT SEEK TRACK 00
                         ; COMMAND TO CONTROLLER
         LDA     NEWTK   ;RE-LOAD ORIGINAL TRACK NUMBER
         STA     TRKNO
         JMP     SKT1    ;SEEK TO ORIGINAL TRACK NUMBER
;
;
;
;ROUTINE TO SELECT UNIT FOR TEST
;
UNITS:   LXI     H,MSG32         ;OUTPUT "UNIT=" TO CONSOLE
         CALL    MSG
         CALL    CONI    ;READ CONSOLE INPUT
         CPI     004H    ;UNIT SELECT NUMBER >3?
         JNC     INER2
         RRC             ;NO IT'S O.K AS REQUESTED BY OPERATOR
                         ;SHIFT IT INTO PROPER POSITION
         RRC
         STA     UNIT    ;STORE UNIT SELECT
         CALL    RSTR    ;TRANSMIT A SEEK TRACK 00 COMMAND
                         ; TO CONTROLLER,TO RESTORE THE SELECTED UNIT
         JMP     MNTR
;
;
;
;SET DATA PATTERN ROUTINE
;
PATT:    LXI     H,MSG13         ;OUTPUT "PATTERN=" TO CONSOLE
         CALL    MSG
         LXI     D,128   ;SET BYTE COUNT "FILL" TO 128 DECIMAL
         CALL    CONI    ;READ CONSOLE KEY-BOARD INPUT
                         ; FOR DESIRED DATA PATTERN
         STA     DBUF    ;STORE DATA BYTE TO FREE-UP A REG.
         LXI     H,WBUF  ;POINT THE DATA PATTERNS AT THE WRITE BUFFER
PATT1:   LDA     DBUF    ;GET THE DESIRED PATTERN FROM STORAGE
         MOV     M,A     ;LOAD UP THE WRITE BUFFER
         INX     H
         DCX     D
         MOV     A,D
         ORA     E
         JNZ     PATT1   ;LOOP UNTIL WRITE BUFFER IS FULL
         JMP     MNTR    ;GO TO MONITOR WHEN DONE
;
;
;
;OPERATOR INPUT ERROR MESSAGE ROUTINES
;
INER1:   LXI     H,MSG4  ;OUTPUT "INVALID TRACK NUMBER " TO CONSOLE
INERA:   CALL    MSG
         CALL    CRLF
         JMP     MNTR
;
INER2:   CALL    CRLF
         LXI     H,MSG33         ;OUTPUT "INVALID UNIT NUMBER" TO CONSOLE
         JMP     INERA
;
INER3:   CALL    CRLF
         LXI     H,MSG37         ;OUTPUT "INVALID SECTOR NUMBER" TO CONSOLE
         JMP     INERA
;
;
;
;SUBROUTINE TO GET UNIT ADDRESS INTO PROPER
; POSITION FOR CONSOLE OUTPUT
;
AUNIT:   LDA     UNIT    ;GET UNIT ADDRESS FROM STORAGE
         RLC             ;ROTATE LEFT 2 PLACES TO GET
                         ; BYTE INTO PROPER POSITION
         RLC
         RET             ;GO BACK TO WHERE YOU CAME FROM
;
;
;
;READ INTO BUFFER FROM PRESENT TRACK

;
RDT:     LXI     H,MSG28         ;OUTPUT "READ SECTOR=" TO CONSOLE
         CALL    MSG
         LXI     H,MSG12
         CALL    MSG
         CALL    CONI    ;READ KEY-BOARD INPUT FROM CONSOLE
                         ; TO GET DESIRED SECTOR NUMBER
         ADI     000H    ;TEST FOR INVALID SECTOR NUMBER
         JZ      INER3   ;GO PRINT "INVALID SECTOR NUMBER",
                         ; IF SO 
         CPI     01BH    ;TEST FOR INVALID SECTOR NUMBER
         JNC     INER3
         STA     SECNO   ;MUST BE GOOD  STORE IT 
         LXI     H,MSG46         ;OUTPUT "CRC ONLY?" TO CONSOLE
         CALL    MSG
         CALL    CECHO   ;GO GET KEY-BOARD INPUT
         CPI     'Y'     ;READ CRC REQUESTED?
         JZ      CRC1    ;IF YES,STORE READ CRC COMMAND
         CPI     'N'     ;IF READ DATA AND CRC,CLEAR CRC COMMAND
         JZ      CRC2
         JMP     LER     ;OUTPUT "?" TO CONSOLE
CRC1:    MVI     A,007H  ;STORE READ CRC COMMAND
         JMP     CRC3
CRC2:    XRA     A       ;CLEAR READ CRC COMMAND
CRC3:    STA     RDCRC
RDTS:    CALL    XMTUS   ;TRANSMIT UNIT/SECTOR TO CONTROLLER
         LXI     D,128
         CALL    CLRER   ;GO CLEAR ERROR FLAGS
RDT1A:   XRA     A       ;CLEAR "SAVED STATUS"
         STA     SAVE
         LDA     RDCRC   ;GO GET STATE OF READ CRC COMMAND
         ANI     007H    ;IS IT A READ CRC COMMAND?
         JZ      RDT1    ;IF NOT,GO READ DATA AND CRC 
         CALL    XMITW
         IN      DATAI   ;EXAMINE STATUS FROM CONTROLLER
         STA     SAVE    ;SAVE POSSIBLE CRC ERROR STATUS
         ANI     008H    ;CRC ERROR?
         CNZ     CLRER   ;CLEAR IT IF SO 
         IN      DATAI   ;ONE MORE TIME FOR THE STATUS
         ANI     080H    ;DELETED DATA MARK?
         CNZ     CLRER   ;CLEAR IT ALSO
         JMP     RDT3    ;GO TEST IT 
RDT1:    MVI     A,003H  ;TRANSMIT READ COMMAND TO CONTROLLER
         CALL    XMITW
         IN      DATAI   ;EXAMINE STATUS FROM CONTROLLER
         STA     SAVE    ;SAVE POSSIBLE CRC ERROR STATUS
         ANI     008H    ;CRC ERROR?
         CNZ     CLRER   ;CLEAR IT IF SO 
         IN      DATAI   ;AND YET ANOTHER TIME FOR THE STATUS 
         ANI     080H    ;NOW A DELETED DATA MARK?
         CNZ     CLRER   ;CLEAR IT AGAIN   
         LXI     H,RBUF  ;SET H&L REGS. T0 POINT TO FIRST
                         ; LOCATION IN READ BUFFER
         MVI     A,040H  ;OUTPUT A READ BUFFER COMMAND,
                         ; TO CONTROLLER
         CALL    XMT4X
         MOV     M,B
         INX     H
         DCX     D
RDT2:    MVI     A,041H  ;GET A DATA BYTE FROM CONTROLLER
         CALL    XMT4X
         MOV     M,B     ;STUFF IT INTO THE READ BUFFER
         INX     H       ;BUMP THE POINTER TO THE NEXT LOCATION,
                         ; IN THE READ BUFFER
         DCX     D       ;-1 TO THE BYTE COUNT
         MOV     A,D     ;ARE WE DONE MOVING IN ALL DATA BYTES?
         ORA     E
         JNZ     RDT2    ;GO FOR MORE,IF NOT DONE 
RDT3:    LDA     SAVE    ;GO GET "SAVED STATUS" FROM STORAGE
         ANI     080H    ;DELETED DATA MARK?
         CNZ     MRK1    ;OUTPUT "DD MARK" TO CONSOLE,IF SO 
         LDA     SAVE
         ANI     008H    ;CRC ERROR?
         CNZ     ERROR   ;GO OUTPUT "CRC ERROR" TO CONSOLE,
                         ; IF CRC ERROR OCCURED 
         LDA     RDCRC   ;GO GET STATE OF READ CRC COMMAND
         ANI     007H
         CZ      RERR    ;GO CHECK FOR DATA COMPARE ERROR,
                         ; IF NOT A READ CRC COMMAND 
RDT4:    CALL    LOOPT   ;GO TEST LOOP COMMAND
         JMP     RDTS
;
;
;
;SUBROUTINE TO TEST DELETED DATA MARK ON READ
;
DDMRK:   IN      DATAI   ;EXAMINE STATUS FROM CONTROLLER
         ANI     080H    ;IS IT A DD MARK?
         RZ
MRK1:    CALL    CLRER   ;CLEAR DD MARK STATUS FROM CONTROLLER
         CALL    CRLF
         LXI     H,MSG44         ;OUTPUT "DD MARK" TO CONSOLE
         CALL    MSG
         CALL    TKSEC   ;OUTPUT TRACK AND SECTOR,THAT THE
                         ; DELETED DATA MARK OCCURED ON 
         RET
;
;
;
;SUBROUTINE TO COMPARE READ DATA TO WRITE DATA,
; AND INDICATE THE FIRST BYTE IN ERROR
;
RERR:    LXI     D,128   ;GO GET BYTE COUNT INTO D&E REGS.
         LXI     B,RBUF  ;SET B&C REGS. TO POINT TO FIRST
                         ; LOCATION IN READ BUFFER
         LXI     H,WBUF  ;SET H&L REGS. TO POINT TO FIRST
                         ; LOCATION IN WRITE BUFFER
RERR1:   MOV     A,M     ;GO GET A BYTE FROM WRITE BUFFER,
                         ; AND STUFF IT INTO A REG.
         STA     SHDBE   ;STORE "S/B" BYTE
         LDAX    B       ;GO GET A BYTE FROM READ BUFFER,
                         ; AND STUFF IT INTO A REG.
         INX     B       ;+1 TO READ BUFFER POINTER
         STA     WAS     ;STORE "WAS" BYTE
         CMP     M       ;COMPARE "WAS" BYTE,TO "S/B" BYTE
         JNZ     RERR3   ;GO PRINT ERROR STATUS,
                         ; IF A MISMATCH OCCURS
         INX     H       ;+1 TO WRITE BUFFER POINTER
         DCX     D       ;-1 TO BYTE COUNT
         XRA     A       ;ZERO OUT A REG.
         ORA     E       ;OR IN E REG.
         STA     DCTR    ;STORE DATA BYTE COUNTER
         JNZ     RERR1   ;GO COMPARE ANOTHER DATA BYTE,
                         ; IF NOT FINISHED WITH ENTIRE RECORD
         RET
;
;
;
;SUBROUTINE TO OUTPUT READ DATA ERROR STATUS TO CONSOLE
;
RERR3:   MOV     A,L     ;SAVE POINTER FOR DATA BYTE COUNTER
         STA     DCTR
         LXI     H,MSG5  ;OUTPUT "READ DATA ERROR" TO CONSOLE
         CALL    MSG
RERR5:   MVI     C,007H  ;RING THAT BELL 
         CALL    CO
         LXI     H,MSG15         ;OUTPUT ERROR MESSAGE HEADER TO CONSOLE
         CALL    MSG
         CALL    CRLF
         CALL    SP1
         CALL    AUNIT   ;OUTPUT ACTUAL UNIT NUMBER TO CONSOLE
         CALL    BYTEO
         CALL    SP3
         LDA     TRKNO   ;OUTPUT TRACK NUMBER TO CONSOLE
         CALL    BYTEO
         CALL    SP4
         LDA     SECNO   ;OUTPUT SECTOR NUMBER TO CONSOLE
         CALL    BYTEO
         CALL    SP5
         LDA     DCTR    ;OUTPUT BYTE NUMBER IN ERROR TO CONSOLE
         CALL    BYTEO
         CALL    SP3
         LDA     WAS     ;OUTPUT "WAS" DATA BYTE TO CONSOLE
         CALL    BYTEO
         CALL    SP2
         LDA     SHDBE   ;OUTPUT "S/B" DATA BYTE TO CONSOLE
         CALL    BYTEO
         CALL    CRLF
         LHLD    ECTR    ;UPDATE THE ERROR COUNTER TO PRESENT TRACK
         INX     H
         SHLD    ECTR
         RET
;
;
;
;WRITE FROM BUFFER TO PRESENT TRACK
;
WRT:     LXI     H,MSG27         ;OUTPUT "WRITE SECTOR=" TO CONSOLE
         CALL    MSG
         LXI     H,MSG12
         CALL    MSG
         CALL    CONI    ;READ KEY-BOARD INPUT FROM CONSOLE
                         ; TO GET DESIRED SECTOR NUMBER
         ADI     000H    ;TEST FOR INVALID SECTOR NUMBER
         JZ      INER3   ;GO PRINT "INVALID SECTOR NUMBER",
                         ; IF SO 
         CPI     01BH    ;TEST FOR INVALID SECTOR NUMBER
         JNC     INER3
         STA     SECNO   ;MUST BE GOOD  STORE IT 
         CALL    WPRT    ;HAS THE OPERATOR REQUESTED WRITE PROTECTION?
         JNZ     WRT4    ;BAIL-OUT ,IF SO 
         IN      DATAI   ;GET STATUS FROM CONTROLLER
         ANI     010H    ;MASK WRITE PROTECT BIT
         JNZ     WRT4    ;IS THE DISKETTE WRITE PROTECTED?
         LXI     H,MSG43         ;OUTPUT "DELETE DATA?" TO CONSOLE
         CALL    MSG
         CALL    CECHO   ;GO GET KEY-BOARD INPUT
         CPI     'Y'     ;DELETED DATA REQUESTED?
         JZ      DEL1    ;IF YES,GO STORE WRITE DELETED
                         ; DATA MARK COMMAND
         CPI     'N'     ;IF NO,CLEAR WRITE DELETED DATA
                         ; MARK COMMAND
         JZ      DEL2
         JMP     LER     ;OUTPUT "?" TO CONSOLE
DEL1:    MVI     A,00FH  ;STORE WRITE DD MARK
         JMP     DEL3
DEL2:    XRA     A       ;ZERO OUT A REG.
DEL3:    STA     DELET
WRT1:    CALL    XMTUS   ;TRANSMIT UNIT/SECTOR TO CONTROLLER
         LXI     D,128
         CALL    CLRER   ;GO CLEAR ERROR FLAGS
         LXI     H,WBUF  ;SET H&L REGS. TO POINT TO FIRST
                         ; LOCATION IN WRITE BUFFER
WRT2:    MOV     A,M     ;GET A DATA BYTE FROM WRITE BUFFER
         OUT     DATAO   ;SPIT IT OUT TO THE CONTROLLER 
         MVI     A,031H
         CALL    XMIT
         INX     H       ;BUMP THE WRITE BUFFER POINTER
         DCX     D       ;-1 TO THE BYTE COUNT
         MOV     A,D     ;HAVE ALL DATA BYTES BEEN SENT?
         ORA     E
         JNZ     WRT2    ;GO SEND SOME MORE,IF NOT DONE 
         LDA     DELET   ;GO GET DELETED DATA MARK COMMAND
         ANI     00FH    ;IS IT A WRITE DD MARK COMMAND?
         JNZ     WRT2A   ;WRITE DD MARK,IF SO 
         MVI     A,005H  ;OUTPUT WRITE COMMAND TO CONTROLLER
WRT2A:   CALL    XMITW
         IN      DATAI   ;EXAMINE STATUS FROM CONTROLLER
         ANI     008H    ;CRC ERROR?
         CNZ     ERROR   ;GO OUTPUT "CRC ERROR" TO CONSOLE,
                         ; IF CRC ERROR OCCURED 
WRT3:    CALL    LOOPT   ;GO TEST LOOP COMMAND
         JMP     WRT1
WRT4:    CALL    WPMSG   ;OUTPUT "WRITE PROTECT" TO CONSOLE
         JMP     MNTR
;
;
;SUBROUTINE TO INDICATE CRC ERROR,AND CLEAR ERROR FLAGS
;
ERROR:   LXI     H,MSG39         ;OUTPUT "CRC ERROR" TO CONSOLE
         CALL    MSG
         CALL    CLRER   ;CLEAR ERROR FLAGS
         RET
;
;
;
;SUBROUTINE TO CLEAR CONTROLLER AND/OR ERROR FLAGS
;
SCLR:    MVI     A,081H  ;TRANSMIT CLEAR CONTROLLER COMMAND
                         ; TO CONTROLLER
         CALL    XMIT
CLRER:   MVI     A,00BH  ;OUTPUT CLEAR ERROR FLAGS TO CONTROLLER
         CALL    XMIT
         RET
;
;
;
VRFYS:   CALL    SEC01   ;INITIALIZE FOR SECTOR 01
         LXI     D,128
         CALL    VRFYR   ;GO CHECK CRC ON A SECTOR
VRFY1:   CALL    INCTN   ;INCREMENT TRACK NUMBER
         JZ      VRFY2   ;GO DO A RESTORE,AND ATTEMPT TO
                         ; CHECK UPPER SURFACE,IF ALL TRACKS
                         ; HAVE BEEN CHECKED ON THE LOWER 
         CALL    XMTTK   ;TRANSMIT LOAD TRACK COMMAND
                         ; TO CONTROLLER
         CALL    SKTRK   ;TRANSMIT SEEK COMMAND,
                         ; TO CONTROLLER
         JMP     VRFYS   ;GO CHECK SECTORS ON A NEW TRACK 
VRFY2:   CALL    RSTR    ;TRANSMIT SEEK TRACK 00 COMMAND,
                         ; TO CONTROLLER
         JMP     FORME   ;EXIT TEST HERE,AND GO TO MONITOR
                         ; AFTER CLEARING FORMAT MODE
;
;
;
;ROUTINE TO VERIFY (READ CRC) ALL TRACKS/SECTORS
;
VRFYT:   LXI     H,MSG49         ;OUTPUT "VERIFY SINGLE TRACK?" TO CONSOLE
         CALL    MSG
         CALL    CECHO   ;GET A KEY-BOARD INPUT
         CPI     'Y'
         JZ      VRF1
         CPI     'N'
         JZ      VRF5
         JMP     LER     ;OOPS 
VRF1:    CALL    SP2
         LXI     H,MSG54         ;OUTPUT "TRACK=" TO CONSOLE
         CALL    MSG
         CALL    CONI
         STA     NEWTK   ;SAVE THE CONSOLE INPUT
         CPI     04DH
         JNC     INER1   ;OOPS 
VRF2:    CALL    RWCI    ;INITIALIZE FOR VERIFY
         LXI     D,128
         LDA     NEWTK
         STA     TRKNO
         CPI     000H    ;TRACK 00 REQUESTED?
         JNZ     VRF3
         CALL    RSTR    ;YES  DO A SEEK TRACK 00
         JMP     VRF4
VRF3:    CALL    XMTTK   ;TRANSMIT TRACK NUMBER,
                         ; TO CONTROLLER
         CALL    SKTRK   ;TRANSMIT SEEK TRACK,
                         ; TO CONTROLLER
VRF4:    CALL    SEC01   ;INITIALIZE FOR SECTOR 01
         LXI     D,128
         CALL    VRFYR   ;READ CRC'S ON THIS TRACK,ALL SECTORS 
         CALL    SEC01   ;RE-INITIALIZE TO SECTOR 01
         CALL    LOOPT
         JMP     VRF2
VRF5:    CALL    CRLF
         MVI     A,001H  ;SET VERIFY COMMAND
         STA     VFYCM
VRF6:    CALL    RWCI    ;INITIALIZE FOR VERIFY
         JMP     VRFYS   ;GO VERIFY ALL TRACKS AND SECTORS
;
;
;
;
;
NFORM:   CALL    SP1
         LXI     H,MSG35         ;OUTPUT "WRITE PROTECT" TO CONSOLE
         CALL    MSG
FORME:   CALL    LOOPT   ;GO TEST LOOP COMMAND
         JMP     VRF6    ;GO RE-VERIFY ALL TRACKS/SECTORS,
                         ; IF REQUESTED 
;
;
;
;SUBROUTINE TO VERIFY (READ CRC) A TRACK IN FORMAT TEST
;
VRFYR:   LDA     TRIES   ;GET RETRIES
         STA     RETRY   ;STORE IN RETRY COUNTER
         CALL    KBINT   ;GO TEST FOR KEY-BOARD INTERRUPT
         CALL    TCRCE   ;GO TEST FOR POSSIBLE CRC ERROR
         CALL    VERSEC  ;GO CHECK FOR PROPER SECTOR NUMBER
         RZ              ;SEEK TO NEXT TRACK,IF DONE 
         CALL    XMTUS   ;WERE NOT FINISHED WITH THIS TRACK YET 
                         ; GO CHECK ANOTHER SECTOR 
         JMP     VRFYR
;
;
;
;SUBROUTINE TO READ AND TEST FOR CRC ERRORS
;
TCRCE:   MVI     A,007H  ;TRANSMIT READ CRC TO CONTROLLER
         CALL    XMITW
CRCER:   IN      DATAI   ;GET STATUS FROM CONTROLLER
         ANI     008H    ;CRC ERROR?
         JNZ     ERTRY   ;GO DO RETRIES,IF CRC ERROR HAS OCCURED 
         RET
ERTRY:   CALL    CLRER   ;TRANSMIT CLEAR ERROR FLAGS,
                         ; TO CONTROLLER
         LXI     H,MSG39         ;OUTPUT "CRC ERROR" TO CONSOLE
         CALL    MSG
         CALL    TKSEC   ;OUTPUT TRACK AND SECTOR IN ERROR,
                         ; TO THE CONSOLE
         LDA     RETRY   ;GO GET THE ERROR RETRY COUNTER,
                         ; AND UPDATE IT 
         DCR     A
         STA     RETRY
         CPI     000H    ;RETRIES FINISHED?
         JZ      MEDER   ;QUIT IF RETRIES ARE FINISHED,
                         ; THIS IS A NON-RECOVERABLE ERROR 
         JMP     TCRCE   ;WERE NOT DONE YET,TRY AGAIN 
;
;
;
;SUBROUTINE TO OUTPUT TRACK AND SECTOR IN ERROR TO THE CONSOLE
;
TKSEC:   CALL    SP1
         LXI     H,MSG31         ;OUTPUT "UNIT=" TO CONSOLE
         CALL    MSG
         CALL    AUNIT   ;OUTPUT UNIT NUMBER TO CONSOLE
         CALL    BYTEO
         CALL    SP2
         LXI     H,MSG58         ;OUTPUT "TRACK=" TO CONSOLE
         CALL    MSG
         LDA     TRKNO   ;GET THE CURRENT TRACK NUMBER
         CALL    BYTEO   ;OUTPUT IT TO THE CONSOLE
         CALL    SP2
         LXI     H,MSG59         ;OUTPUT "SECTOR=" TO CONSOLE
         CALL    MSG
         LDA     SECNO   ;GO GET THE CURRENT SECTOR NUMBER
         CALL    BYTEO   ;OUTPUT IT TO THE CONSOLE
         RET
;
;
;
;SUBROUTINE TO INDICATE A "HARD ERROR"(MEDIA ERROR),RESTORE,AND QUIT
;
MEDER:   LXI     H,MSG17         ;OUTPUT "MEDIA ERROR" TO CONSOLE
         CALL    MSG
         CALL    RSTR    ;TRANSMIT SEEK TRACK 00 COMMAND,
                         ; TO CONTROLLER
         JMP     MNTR
;
;
;
;
;
;TEST WRITE/READ CONTINUOUS (WRITE AND READ ALL AVAILABLE TRACKS)
;
RWC:     LXI     H,MSG30         ;OUTPUT "TEST WRITE/READ CONTINUOUS"
                         ; TO CONSOLE
         CALL    MSG
RWCP:    MVI     C,007H  ;OUTPUT A "BELL" TO INDICATE THE
                         ; START OF A PASS
         CALL    CO
         LXI     H,MSG1  ;OUTPUT "PASS=" TO CONSOLE
         CALL    MSG
         LHLD    PCTR    ;OUTPUT PASS NUMBER TO CONSOLE
         MOV     A,H
         CALL    BYTEO

         MOV     A,L
         CALL    BYTEO
         LXI     H,MSG16         ;OUTPUT "ERROR TOTAL=" TO CONSOLE
         CALL    MSG
         LHLD    ECTR    ;OUTPUT ERROR NUMBER TO CONSOLE
         MOV     A,H
         CALL    BYTEO
         MOV     A,L
         CALL    BYTEO
         CALL    RWCI    ;GO INITIALIZE FOR WRITE/READ CONTINUOUS
         CALL    WPRT    ;GO GET STATE OF OPERATOR REQUESTED
                         ; WRITE PROTECT
         JNZ     READO   ;GO DO "READ ONLY" IF WRITE PROTECTED 
         IN      DATAI   ;GET STATUS FROM CONTROLLER
         ANI     010H    ;IS THE DISKETTE WRITE PROTECTED?
         JNZ     READO   ;JUMP TO DO READ ONLY ROUTINE
                         ; IF WRITE PROTECT STATUS IS TRUE 
RWCWW:   CALL    CRLF
         LXI     H,MSG2  ;OUTPUT "WRITE" TO CONSOLE
         CALL    MSG
         CALL    CRLF
RWCWS:   CALL    SEC01   ;INITIALIZE FOR SECTOR 01
RWCW:    CALL    KBINT   ;GO TEST FOR KEY-BOARD INTERRUPT
         CALL    RWCW1   ;GO WRITE A SECTOR
         CALL    SECCK   ;GO CHECK FOR PROPER SECTOR NUMBER
         JZ      RWCW4   ;GO TO A NEW TRACK IF FINISHED 
         CALL    XMTUS   ;TRANSMIT LOAD UNIT/SECTOR
                         ; COMMAND,TO CONTROLLER
         JMP     RWCW    ;GO WRITE NEXT SECTOR
RWCW4:   CALL    INCTN   ;INCREMENT TRACK NUMBER
         JZ      RWCW5
         CALL    XMTTK   ;TRANSMIT LOAD TRACK NUMBER
                         ; TO CONTROLLER
         CALL    SKTRK   ;TRANSMIT SEEK COMMAND TO CONTROLLER
         JMP     RWCWS   ;GO WRITE SECTOR ON NEW TRACK NUMBER
RWCW5:   CALL    RSTR    ;TRANSMIT SEEK TRACK 00
                         ; COMMAND TO CONTROLLER
RWCRX:   CALL    KBINT   ;GO CHECK FOR KEY-BOARD INTERRUPT
         LDA     BLOOP   ;GET THE STATE OF THE LOOP COMMAND
         ADI     000H    ;LOOP ON WRITE CONTINUOUS?
         JNZ     RWCE    ;GO DO ANOTHER WRITE PASS IF SO 
RWCRR:   CALL    CRLF
         LXI     H,MSG3  ;OUTPUT "READ" TO CONSOLE
         CALL    MSG
         CALL    CRLF
         CALL    RWCI    ;INITIALIZE TEST FOR READ
RWCRS:   CALL    SEC01   ;INITIALIZE FOR SECTOR 01
RWCR1:   CALL    KBINT   ;GO TEST FOR KEY-BOARD INTERRUPT
         LXI     D,128
         CALL    RWCR2   ;GO READ A SECTOR
         CALL    SECCK   ;GO CHECK FOR PROPER SECTOR NUMBER
         JZ      RWCR6   ;GO TO A NEW TRACK IF FINISHED 
         CALL    XMTUS   ;TRANSMIT LOAD UNIT/SECTOR COMMAND
                         ; TO CONTROLLER,WERE NOT DONE WITH
                         ; THIS TRACK YET 
         JMP     RWCR1   ;GO READ NEXT SECTOR
RWCR6:   CALL    INCTN   ;INCREMENT TRACK NUMBER
         JZ      RWCR7
         CALL    XMTTK   ;TRANSMIT LOAD TRACK COMMAND
                         ; TO CONTROLLER
         CALL    SKTRK   ;TRANSMIT SEEK COMMAND TO CONTROLLER
         JMP     RWCRS   ;GO READ SECTOR ON NEW TRACK NUMBER
RWCR7:   CALL    RSTR    ;TRANSMIT SEEK TRACK 00
                         ; COMMAND TO CONTROLLER
RWCE:    LHLD    PCTR    ;GO GET PASS COUNTER FROM STORAGE,
                         ; AND UPDATE IT
         INX     H       ;INCREMENT PASS COUNTER
         SHLD    PCTR    ;STORE NEW PASS NUMBER
         JMP     RWCP    ;GO DO THE NEXT PASS
;
;
;
READO:   CALL    SP1
         LXI     H,MSG35         ;OUTPUT "WRITE PROTECT" TO CONSOLE
         CALL    MSG
         JMP     RWCRX   ;GO DO A READ PASS ONLY 
;
;
;
;SUBROUTINE TO WRITE A SECTOR IN WRITE/READ CONTINUOUS TEST
;
RWCW1:   CALL    WSEC    ;SET-UP FOR WRITE SECTOR
         MVI     A,005H  ;TRANSMIT WRITE COMMAND TO CONTROLLER
         CALL    XMITW
         CALL    WSTAT   ;GO FIND OUT STATUS OF WRITE
         RZ
         CALL    WERR    ;OOPS CRC ERROR   
         JMP     RWCWS   ;GO TRY AGAIN
;
;
;
;SUBROUTINE TO SET-UP WRITE SECTOR
;
WSEC:    CALL    PTKSC   ;FILL WRITE BUFFER WITH ALTERNATE
                         ; TRACK AND SECTOR NUMBER,AS DATA
         LXI     D,128
         LXI     H,WBUF  ;SET H&L REGS. TO POINT TO FIRST
                         ; LOCATION IN WRITE BUFFER
WSEC1:   MOV     A,M     ;LOAD A BYTE FROM WRITE BUFFER
                         ; FOR OUTPUT TO CONTROLLER
         OUT     DATAO   ;OUTPUT IT 
         MVI     A,031H
         CALL    XMIT
         INX     H       ;BUMP THE POINTER TO THE NEXT
                         ; LOCATION IN THE WRITE BUFFER
         DCX     D       ;-1 TO BYTE COUNT
         MOV     A,D     ;ZERO OUT A REG.
         ORA     E       ;OR IN E REG.
         JNZ     WSEC1   ;GO OUTPUT ANOTHER BYTE IF ALL THE DATA
                         ; HAS NOT BEEN TRANSMITTED TO
                         ; THE CONTROLLER
         RET
;
;
;
;SUBROUTINE TO CHECK STATUS OF WRITE SECTOR
;


WSTAT:   IN      DATAI   ;GO GET STATUS FROM CONTROLLER
         ANI     008H    ;CRC ERROR?FIND OUT AT THE RETURN
         RET
;
;
;
;SUBROUTINE TO INDICATE A WRITE ERROR
;
WERR:    LXI     H,MSG39         ;YES OUTPUT "CRC ERROR" TO CONSOLE
         CALL    MSG
         LXI     H,MSG2  ;OUTPUT "WRITE" TO CONSOLE
         CALL    MSG
         CALL    CLRER   ;TRANSMIT CLEAR ERROR FLAGS,
                         ; TO CONTROLLER
         CALL    TKSEC   ;OUTPUT TRACK AND SECTOR IN ERROR TO CONSOLE
         LHLD    ECTR    ;UPDATE THE ERROR COUNTER
         INX     H
         SHLD    ECTR
         RET
;
;
;
;SUBROUTINE TO READ A SECTOR IN WRITE/READ CONTINUOUS TEST
;
RWCR2:   XRA     A       ;CLEAR OUT OLD "SAVE" STATUS
         STA     SAVE
RWCR3:   MVI     A,003H  ;TRANSMIT READ COMMAND
                         ; TO CONTROLLER
         CALL    XMITW
         IN      DATAI   ;GET CONTROLLER STATUS
         STA     SAVE    ;SAVE IT 
         ANI     008H    ;HAVE WE GOT A CRC ERROR?
         CNZ     CLRER   ;GO CLEAR ERROR FLAGS,IF SO 
         CALL    DDMRK   ;GO TEST FOR POSSIBLE DD MARK
         LXI     H,RBUF  ;SET H&L REGS. TO POINT TO FIRST
                         ; LOCATION IN READ BUFFER
         MVI     A,040H  ;TRANSMIT READ BUFFER COMMAND,
                         ; TO CONTROLLER
         CALL    XMT4X
         MOV     M,B
         INX     H
         DCX     D
RWCR4:   MVI     A,041H  ;GET A DATA BYTE 
         CALL    XMT4X
         MOV     M,B     ;STUFF IT INTO THE READ BUFFER
         INX     H       ;BUMP THE READ BUFFER POINTER
         DCX     D       ;-1 TO THE BYTE COUNT
         MOV     A,D     ;IS ALL THE DATA IN?
         ORA     E
         JNZ     RWCR4   ;GO GET ANOTHER DATA BYTE IF NOT DONE 
         CALL    PTKSC   ;FILL WRITE BUFFER WITH ALTERNATE
                         ; TRACK AND SECTOR NUMBER,AS DATA
         CALL    RERR    ;GO DO A BYTE FOR BYTE COMPARE ON DATA,
                         ; FOR POSSIBLE ERROR IN DATA FIELD
                         ; (CRC ERROR STATUS REPORT MAY
                         ; BE BROKEN,AND NOT TELL US)
         LDA     SAVE    ;GO GET "SAVED" STATUS
         ANI     008H    ;CRC ERROR?
         RZ
         LXI     H,MSG39         ;YES OUTPUT "CRC ERROR" TO CONSOLE
         CALL    MSG
         LXI     H,MSG3  ;OUTPUT "READ" TO CONSOLE
         CALL    MSG
         CALL    CLRER   ;CLEAR ANY ERROR
         CALL    TKSEC   ;OUTPUT TRACK AND SECTOR IN ERROR TO CONSOLE
         LHLD    ECTR    ;UPDATE THE ERROR COUNTER
         INX     H
         SHLD    ECTR
         JMP     RWCRS
;
;
;
;
;
;
;
;
;
;SUBROUTINE TO INITIALIZE WRITE/READ CONTINUOUS
;
RWCI:    CALL    SEC01   ;INITIALIZE FOR SECTOR 01
         CALL    XMTUS   ;TRANSMIT LOAD UNIT/SECTOR
                         ; TO CONTROLLER
         MVI     A,081H  ;TRANSMIT CLEAR COMMAND
                         ; TO CONTROLLER
         CALL    XMIT
         CALL    RSTR    ;TRANSMIT SEEK TO TRACK 00
                         ; COMMAND TO CONTROLLER
         CALL    CLRER   ;TRANSMIT CLEAR ERROR FLAGS
                         ; TO CONTROLLER
         RET
;
;
;
;
;
;
;SYSTEM TEST (WRITES ALL TRACKS,READS TRACKS ON RANDOM SEEKS,UNITS 00 AND 01)
;
SYS:     LXI     H,MSG57         ;OUTPUT "SYSTEM TEST"
                         ; TO CONSOLE
         CALL    MSG
SYSS:    CALL    RGEN    ;SET-UP THE RANDOM TRACK NUMBER TABLE
         XRA     A       ;SET UNIT SELECT TO 00
         STA     UNIT
         MVI     C,007H  ;OUTPUT A "BELL" TO INDICATE THE
                         ; START OF A PASS
         CALL    CO
         LXI     H,MSG1  ;OUTPUT "PASS=" TO CONSOLE
         CALL    MSG
         LHLD    PCTR    ;OUTPUT PASS NUMBER TO CONSOLE
         MOV     A,H
         CALL    BYTEO

         MOV     A,L
         CALL    BYTEO
         LXI     H,MSG16         ;OUTPUT "ERROR TOTAL=" TO CONSOLE
         CALL    MSG
         LHLD    ECTR    ;OUTPUT ERROR NUMBER TO CONSOLE
         MOV     A,H
         CALL    BYTEO
         MOV     A,L
         CALL    BYTEO
SYSWI:   CALL    RWCI    ;GO INITIALIZE FOR SYSTEM TEST
         IN      DATAI   ;GET STATUS FROM THE SELECTED UNIT
         ANI     010H    ;MASK WRITE PROTECT
         JNZ     MNTR    ;GO TO THE COMMAND MONITOR,IF WRITE PROTECTED 
         CALL    UFLIP   ;SELECT THE NEXT UNIT
         JNZ     SYSWI   ;RESTORE AND CHECK IT ALSO 
         CALL    WPRT    ;CHECK FOR OPERATOR REQUESTED WRITE PROTECTION
         JNZ     SYSRO   ;DO "READ ONLY" PASSES IF REQUESTED 
SYSW:    CALL    CRLF
         LXI     H,MSG2  ;OUTPUT "WRITE" TO CONSOLE
         CALL    MSG
         CALL    CRLF
SYSW1:   CALL    SEC01   ;INITIALIZE FOR SECTOR 01
         CALL    WMK     ;WRITE A DELETED DATA MARK ON SECTOR 01 ONLY 
         CALL    SECCK   ;BUMP THE SECTOR NUMBER
         CALL    XMTUS   ;TRANSMIT NEXT SECTOR TO CONTROLLER
SYSW2:   CALL    KBINT   ;GO TEST FOR KEY-BOARD INTERRUPT
         CALL    SYSWS   ;GO WRITE A SECTOR
         CALL    SECCK   ;GO CHECK FOR PROPER SECTOR NUMBER
         JZ      SYSW3   ;GO TO A NEW TRACK IF FINISHED 
         CALL    XMTUS   ;TRANSMIT LOAD UNIT/SECTOR
                         ; COMMAND,TO CONTROLLER
         JMP     SYSW2   ;GO WRITE NEXT SECTOR
SYSW3:   CALL    UFLIP   ;SELECT NEW UNIT
         JNZ     SYSW1   ;DO A WRITE ON UNIT 01,IF NOT UNIT 00 
         CALL    INCTN   ;INCREMENT TRACK NUMBER
         JZ      SYSW5   ;GO DO A RESTORE,AND ATTEMPT TO
                         ; WRITE UPPER SURFACE,IF ALL TRACKS HAVE BEEN
                         ; WRITTEN ON THE LOWER 
SYSW4:   CALL    XMTUS   ;TRANSMIT LOAD UNIT/SECTOR COMMAND,
                         ; TO CONTROLLER
         CALL    XMTTK   ;TRANSMIT LOAD TRACK NUMBER TO CONTROLLER
                         ; COMMAND TO CONTROLLER
         CALL    SKTRK   ;TRANSMIT SEEK COMMAND TO CONTROLLER
         CALL    UFLIP   ;SELECT NEW UNIT
         JNZ     SYSW4   ;SEEK TO NEXT TRACK ON UNIT 01,
                         ; IF NOT UNIT 00 
         JMP     SYSW1   ;GO WRITE SECTOR ON NEW TRACK NUMBER
SYSW5:   CALL    RSTR    ;TRANSMIT SEEK TRACK 00 COMMAND,
                         ; TO CONTROLLER
         CALL    UFLIP   ;SELECT NEW UNIT
SYSR:    CALL    KBINT   ;GO CHECK FOR KEY-BOARD INTERRUPT
         LDA     BLOOP   ;GET THE STATE OF THE LOOP COMMAND
         ADI     000H    ;LOOP ON WRITE CONTINUOUS?
         JNZ     SYSE    ;GO DO ANOTHER WRITE PASS IF SO 
SYSR1:   CALL    CRLF
         LXI     H,MSG3  ;OUTPUT "READ" TO CONSOLE
         CALL    MSG
         CALL    CRLF
SYSRI:   CALL    RWCI    ;INITIALIZE TEST FOR READ
         CALL    UFLIP   ;SELECT NEW UNIT
         JNZ     SYSRI   ;DO A RESTORE ON UNIT 01,IF NOT UNIT 00 
SYSR2:   LXI     D,128
         CALL    RMK     ;CHECK FOR A DELETED DATA MARK ON SECTOR 01 ONLY 
         CALL    SEC01   ;INITIALIZE FOR SECTOR 01
SYSR3:   CALL    KBINT   ;GO TEST FOR KEY-BOARD INTERRUPT
         LXI     D,128
         CALL    SYSRS   ;GO READ A SECTOR
         CALL    SECCK   ;GO CHECK FOR PROPER SECTOR NUMBER
         JZ      SYSR4   ;GO TO A NEW TRACK IF FINISHED 
         CALL    XMTUS   ;TRANSMIT LOAD UNIT/SECTOR COMMAND,
                         ; TO CONTROLLER
         JMP     SYSR3   ;GO READ NEXT SECTOR
SYSR4:   CALL    UFLIP   ;SELECT NEW UNIT
         CALL    RND     ;GO GET A RANDOM TRACK NUMBER FROM TABLE
         CALL    SEC01   ;SET SECTOR NUMBER TO 01
         CALL    XMTUS   ;TRANSMIT LOAD UNIT/SECTOR COMMAND,
                         ; TO CONTROLLER
         CALL    XMTTK   ;TRANSMIT LOAD TRACK COMMAND
                         ; TO CONTROLLER
         CALL    SKTRK   ;TRANSMIT SEEK COMMAND TO CONTROLLER
         JMP     SYSR2   ;GO READ SECTOR ON NEW TRACK NUMBER
;
;
;
;SUBROUTINE TO "FLIP" UNIT SELECTS
;
UFLIP:   LDA     UNIT    ;GET THE CURRENT STATE OF UNIT SELECT
         CMA             ;COMPLEMENT IT 
         ANI     040H    ;MASK FOR UNIT SELECT 01
         STA     UNIT    ;PUT NEW UNIT SELECT BACK 
         CPI     000H    ;UNIT SELECT 00?
         RET             ;FIND OUT AT THE RETURN   
;
;
;
;ROUTINE TO GENERATE RANDOM TRACK NUMBERS INTO TABLE
;
RGEN:    LXI     D,256   ;SET RANDOM TABLE LENGTH TO 256
         LXI     H,RANTK         ;POINT AT RANDOM TRACK NUMBER TABLE
         MOV     A,M     ;GET TRASH FROM MEMORY
CHEW:    STA     SECNO   ;START CHEWING 
         MOV     B,A     ;PUT IT IN B REG.
         LDA     TRKNO   ;GET THE CURRENT TRACK NUMBER
         XRA     B       ;SCREW IT UP 
         RLC
         RLC
         RLC
CMP4D:   CPI     04DH    ;> TRACK 76?
         JC      STTRK   ;IF NOT,STORE IT AS NEW TRACK NUMBER
         RAR             ;SCREW IT UP SOME MORE 
         JMP     CMP4D
STTRK:   STA     TRKNO   ;THE NEW TRACK NUMBER 
CMP1B:   CPI     01BH    ;> SECTOR 26?
         JC      STSEC   ;IF NOT,STORE IT AS NEW SECTOR NUMBER
         RAR             ;SCREW THIS ONE UP ALSO 
         JMP     CMP1B
STSEC:   STA     SECNO   ;THE NEW SECTOR NUMBER 
         LDA     TRKNO   ;STUFF THE TRACK NUMBER INTO THE TABLE
         CPI     000H    ;HAS TRACK 00 BEEN GENERATED?
         JNZ     STMEM
         MVI     A,001H  ;YES  CHANGE IT TO TRACK 01 
STMEM:   MOV     M,A
         INX     H       ;BUMP THE POINTER
         DCX     D       ;DECREMENT THE LENGTH
         MOV     A,E
         ORA     D
         JNZ     CHEW    ;GO SCREW-UP THE TRACK NUMBER SOME
                         ; MORE,IF THE TABLE IS NOT FULL 
         LXI     H,256   ;SET THE RANDOM SEEK ITERATIONS COUNTER
                         ; TO 256 RANDOM SEEKS
         SHLD    RCTR
         LXI     H,RANTK         ;SET-UP THE TABLE POINTER
         SHLD    RPNT
         RET
;
;
;
;
;
;
;ROUTINE TO GET A RANDOM TRACK NUMBER FROM THE TABLE
;
RND:     LHLD    RCTR    ;GET THE CURRENT SEEK COUNT
         XCHG            ;PUT IT IN THE D&E REGS.
         LHLD    RPNT    ;POINT AT THE RANDOM TRACK TABLE
         MOV     A,M     ;GET A TRACK NUMBER
         STA     TRKNO   ;SAVE IT 
         INX     H       ;BUMP THE POINTER
         SHLD    RPNT    ;SAVE IT ALSO 
         DCX     D       ;-1 TO THE RANDOM SEEK ITERATIONS COUNT
         XCHG            ;SWAP 
         SHLD    RCTR    ;SAVE THE COUNT
         XCHG            ;SWAP AGAIN 
         MOV     A,E
         ORA     D
         RNZ             ;GO DO ANOTHER SEEK,IF 256 SEEKS
                         ; NOT FINISHED 
SYSE:    LHLD    PCTR    ;UPDATE THE PASS COUNTER
         INX     H
         SHLD    PCTR
         LXI     SP,STACK        ;SET THE STACK POINTER TO RESOLVE
                                 ; THE UNSATISFIED CALL TO "RND"
                                 ; AFTER 256 RANDOM SEEKS 
         JMP     SYSS    ;GO DO THE NEXT PASS
;
;
;
;
;
;
SYSRO:   CALL    SP1
         LXI     H,MSG35         ;OUTPUT "WRITE PROTECT" TO CONSOLE
         CALL    MSG
         JMP     SYSR    ;GO DO A READ PASS ONLY 
;
;
;
;
;
;
;SUBROUTINE TO WRITE A SECTOR IN SYSTEM TEST
;
SYSWS:   CALL    WSEC    ;SET-UP TO WRITE A SECTOR
         MVI     A,005H  ;TRANSMIT WRITE COMMAND TO CONTROLLER
         CALL    XMITW
         CALL    WSTAT   ;GO CHECK STATUS OF WRITE
         RZ
         CALL    WERR    ;OOPS CRC ERROR GOT US AGAIN 
         JMP     SYSW1   ;GO TRY AGAIN 
;
;
;
;
;
;
;SUBROUTINE TO READ A SECTOR IN SYSTEM TEST
;
SYSRS:   XRA     A       ;CLEAR OUT OLD "SAVE" STATUS
         STA     SAVE
         MVI     A,003H  ;TRANSMIT READ COMMAND
                         ; TO CONTROLLER
         CALL    XMITW
         IN      DATAI   ;GET CONTROLLER STATUS
         STA     SAVE    ;SAVE IT 
         ANI     008H    ;HAVE WE GOT A CRC ERROR?
         CNZ     CLRER   ;GO CLEAR ERROR FLAGS,IF SO 
         IN      DATAI   ;GET THE STATUS
         ANI     080H    ;DELETED DATA MARK?
         CNZ     CLRER   ;CLEAR THE ERROR FLAGS,IF SO 
         LXI     H,RBUF  ;SET H&L REGS. TO POINT TO FIRST
                         ; LOCATION IN READ BUFFER
         MVI     A,040H  ;TRANSMIT READ BUFFER COMMAND,
                         ; TO CONTROLLER
         CALL    XMT4X
         MOV     M,B
         INX     H
         DCX     D
RSEC:    MVI     A,041H  ;GET A DATA BYTE 
         CALL    XMT4X
         MOV     M,B     ;STUFF IT INTO THE READ BUFFER
         INX     H       ;BUMP THE READ BUFFER POINTER
         DCX     D       ;-1 TO THE BYTE COUNT
         MOV     A,D     ;IS ALL THE DATA IN?
         ORA     E
         JNZ     RSEC    ;GO GET ANOTHER DATA BYTE IF NOT DONE 
         CALL    PTKSC   ;FILL WRITE BUFFER WITH ALTERNATE
                         ; TRACK AND SECTOR NUMBER,AS DATA
         CALL    RERR    ;GO DO A BYTE FOR BYTE COMPARE ON DATA,
                         ; FOR POSSIBLE ERROR IN DATA FIELD
                         ; (CRC ERROR STATUS REPORT MAY
                         ; BE BROKEN,AND NOT TELL US)
         LDA     SAVE    ;GO GET "SAVED" STATUS
         ANI     008H    ;CRC ERROR?
         RZ
         LXI     H,MSG39         ;YES OUTPUT "CRC ERROR" TO CONSOLE
         CALL    MSG
         LXI     H,MSG3  ;OUTPUT "READ" TO CONSOLE
         CALL    MSG
         CALL    CLRER   ;CLEAR ANY ERROR
         CALL    TKSEC   ;OUTPUT TRACK AND SECTOR IN ERROR TO CONSOLE
         LHLD    ECTR    ;UPDATE THE ERROR COUNTER
         INX     H
         SHLD    ECTR
         JMP     SYSR3
;
;
;
;SUBROUTINE TO WRITE AND CHECK A DELETED DATA MARK
;
WMK:     CALL    WSEC    ;SET-UP TO WRITE A SECTOR
         MVI     A,00FH  ;TRANSMIT A WRITE DELETED DATA MARK COMMAND
                         ; TO CONTROLLER
         CALL    XMITW
         CALL    WSTAT   ;CHECK WRITE STATUS
         JZ      WMKR    ;GO READ DD MARK AND CRC,IF O.K.
         CALL    WERR    ;OOPS ANOTHER DAMN CRC ERROR 
         JMP     SYSW1
WMKR:    CALL    SEC01   ;INITIALIZE FOR SECTOR 01
         MVI     A,003H  ;TRANSMIT A READ COMMAND TO CONTROLLER
         CALL    XMITW
         IN      DATAI   ;GET THE STATUS
         ANI     008H    ;CRC ERROR?
         JZ      WMK1
         CALL    ERROR   ;YES   
WMK1:    IN      DATAI   ;GET THE STATUS AGAIN
         ANI     080H    ;DD MARK?
         JZ      WMK2
         CALL    CLRER   ;YES,IT'S WHERE IT SHOULD BE 
         RET
WMK2:    CALL    FAIL    ;OOPS  WE ARE MISSING THE DD MARK 
         LXI     H,MSG2  ;OUTPUT "WRITE" TO CONSOLE
         CALL    MSG
         JMP     MKERR
;
;
;
;SUBROUTINE TO READ AND CHECK A DELETED DATA MARK
;
RMK:     CALL    SEC01   ;INITIALIZE FOR SECTOR 01
         MVI     A,003H  ;TRANSMIT A READ COMMAND TO CONTROLLER
         CALL    XMITW
         IN      DATAI   ;GET THE STATUS
         ANI     008H    ;CRC ERROR?
         JZ      RMK1
         CALL    ERROR   ;YES   
RMK1:    IN      DATAI   ;ONE MORE TIME FOR THE STATUS 
         ANI     080H    ;DD MARK?
         JZ      RMK2
         CALL    CLRER   ;YES,IT'S REALLY ON SECTOR 01 
         RET
RMK2:    CALL    FAIL    ;OH  OH  NO DD MARK 
         LXI     H,MSG3  ;OUTPUT "READ " TO CONSOLE
         CALL    MSG
MKERR:   LXI     H,MSG44         ;OUTPUT "DD MARK" TO CONSOLE
         CALL    MSG
         CALL    TKSEC   ;OUTPUT THE TRACK AND SECTOR IN ERROR 
         LHLD    ECTR    ;UPDATE THE ERROR COUNTER
         INX     H
         SHLD    ECTR

         RET
;
;
;
FAIL:    LXI     H,MSG23         ;OUTPUT "FAILED TO" TO CONSOLE
         CALL    MSG
         RET
;
;
;
;
;
;
;SUBROUTINE TO INCREMENT TRACK NUMBER
;
INCTN:   MVI     A,001H  ;SET SECTOR NUMBER TO 01
         STA     SECNO
         LDA     TRKNO   ;GET PRESENT TRACK NUMBER
         INR     A       ;+1 TO TRACK NUMBER
         STA     TRKNO   ;STORE IT 
         CPI     04DH    ;TRACK NUMBER >76 DECIMAL?
         RET             ;FIND OUT AT RETURN 
;
;
;
;SUBROUTINE TO SEEK TO A TRACK
;
SKTRK:   MVI     C,'.'   ;MAKE A TRACK SEEK INDICATOR FOR THE CONSOLE
         CALL    CO
         CALL    KBINT   ;GO CHECK FOR KEY-BOARD INTERRUPT
         LXI     D,128
         MVI     A,009H  ;TRANSMIT SEEK COMMAND,
                         ; TO CONTROLLER
         CALL    XMITW
         IN      DATAI   ;GO GET CONTROLLER STATUS
         ANI     008H    ;CRC ERROR?
         RZ
         CALL    CLRER   ;CLEAR THE CRC ERROR 
         LHLD    ECTR    ;UPDATE THE ERROR COUNTER
         INX     H
         SHLD    ECTR
         LXI     H,MSG39         ;OUTPUT "CRC ERROR" TO CONSOLE
         CALL    MSG
         CALL    SP1
         LXI     H,MSG60         ;OUTPUT "SEEK TO TRACK=" TO CONSOLE
         CALL    MSG
         LDA     TRKNO   ;GO GET CURRENT TRACK NUMBER
         STA     NEWTK   ;SAVE IT 
         CALL    BYTEO   ;OUTPUT IT TO THE CONSOLE 
         CALL    SP2
         LXI     H,MSG31         ;OUTPUT "UNIT=" TO CONSOLE
         CALL    MSG
         CALL    AUNIT   ;OUTPUT UNIT NUMBER TO CONSOLE
         CALL    BYTEO
         CALL    RSTR    ;TRANSMIT SEEK TRACK 00 COMMAND,
                         ; TO CONTROLLER
         LDA     NEWTK   ;O.K NOW PUT IT BACK 
         STA     TRKNO
         JMP     SKTRK   ;LET'S TRY SEEKING TO THIS TRACK AGAIN 
;
;
;
;
;SUBROUTINE TO FILL WRITE BUFFER WITH TRACK AND SECTOR NUMBER DATA
;
PTKSC:   LXI     D,128   ;SET BYTE COUNT FOR CURRENT TRACK POSITION
         LXI     H,WBUF  ;SET-UP POINTER AT WRITE BUFFER
DPAT:    LDA     TRKNO   ;GO GET THE CURRENT TRACK NUMBER
         MOV     M,A     ;PUT IT IN THE WRITE BUFFER
         INX     H       ;BUMP THE WRITE BUFFER POINTER
         DCX     D       ;-1 TO THE BYTE COUNT
         XRA     A       ;ALL THE DATA BEEN MOVED?
         ORA     E
         RZ              ;RETURN IF SO 
         LDA     SECNO   ;GO GET THE CURRENT SECTOR NUMBER
         MOV     M,A     ;PUT IT INTO THE WRITE BUFFER ALSO 
         INX     H       ;BUMP THE WRITE BUFFER POINTER AGAIN 
         DCX     D       ;ANOTHER -1 TO THE BYTE COUNT
         XRA     A       ;DONE YET???
         ORA     E
         JNZ     DPAT    ;WELL,IF NOT DONE,GO LOAD A TRACK NUMBER
         RET             ;AHHHHH  FINALLY DONE  
;
;
;
;SUBROUTINE TO DO 26 SEQUENTIAL SECTORS
;
VERSEC:  LDA     SECNO   ;GET THE CURRENT SECTOR NUMBER
         INR     A
         STA     SECNO   ;AND IT'S BUMPED 
         CPI     01BH    ;ALL DONE FOR 26 SECTORS?
         RET             ;...FIND OUT AT THE RETURN 
;
;
;
;SUBROUTINE TO TRANSMIT THE PROPER SECTOR NUMBER TO THE CONTROLLER
;
SECCK:   JMP     SEC26
;
;
;
;SUBROUTNE TO DO 26 LOGICAL SECTORS
;
SEC26:   CALL    INCSN   ;GO INCREMENT LOGICAL SECTOR NUMBER
         LDA     LSN     ;IS THE LAST SECTOR DONE,FOR 26 LOGICAL SECTORS?
         CPI     01BH
         RET             ;GO FIND OUT AT THE RETURN 
;
;
;
;SUBROUTINE TO INITIALIZE FOR SECTOR 01
;
SEC01:   MVI     A,001H  ;SET SECTOR NUMBER TO 01
         STA     LSN
         CALL    OSCTR   ;INITIALIZE TO LOGICAL SECTOR CONVERSION
         CALL    XMTUS   ;TRANSMIT LOAD UNIT/SECTOR,
                         ; TO CONTROLLER
         RET
;
;
;
;SUBROUTINE TO INCREMENT LOGICAL SECTOR NUMBER
;
INCSN:   LDA     LSN     ;GO GET PRESENT SECTOR NUMBER
         INR     A       ;INCREMENT SECTOR NUMBER
         STA     LSN     ;STORE IT 
         CALL    OSCTR   ;GO DO LOGICAL SECTOR CONVERSION
         RET             ;FIND OUT AT RETURN 
;
;
;
;
;LOGICAL TO PHYSICAL SECTOR NUMBER CONVERSION SUBROUTINE FOR 26 SECTORS
;
OSCTR:   MOV     E,A     ;MOVE LOGICAL SECTOR NUMBER TO E REG.
         MVI     D,000H  ;CLEAR OUT D REG.
         LXI     H,LSTBL-1       ;POINT AT LOGICAL SECTOR TABLE
         DAD     D       ;ADD D&E REGS. TO LOGICAL SECTOR
         MOV     A,M     ;MOVE LOGICAL SECTOR TO A REG.
         STA     SECNO   ;STORE LOGICAL NUMBER IN "SECNO"
         RET
;
;
;
;LOGICAL SECTOR TABLE (SKIPS EVERY 5 SECTORS)
;
LSTBL:   DB      001H
         DB      007H
         DB      00DH
         DB      013H
         DB      019H
         DB      005H
         DB      00BH
         DB      011H
         DB      017H
         DB      003H
         DB      009H
         DB      00FH
         DB      015H
         DB      002H
         DB      008H
         DB      00EH
         DB      014H
         DB      01AH
         DB      006H
         DB      00CH
         DB      012H
         DB      018H
         DB      004H
         DB      00AH
         DB      010H
         DB      016H
;
;
;
;SUBROUTINE TO SEEK TO TRACK 00 (DO A RESTORE)
;
RSTR:    XRA     A       ;ZERO OUT A REG.
         STA     TRKNO   ;SET TRACK NUMBER TO 00
         CALL    XMTUS   ;TRANSMIT LOAD UNIT/SECTOR TO CONTROLLER
         MVI     A,0DH   ;TRANSMIT SEEK TO TRACK 00,
                         ; TO CONTROLLER
         CALL    XMITW
         RET
;
;
;
;ROUTINE TO ALLOW C.E. ALIGNMENT OF THE FLEXIBLE DISK DRIVE
; (NOTE: THIS ROUTINE WILL NOT ALLOW THE OPERATOR TO UTILIZE
;  THIS ROUTINE,UNLESS THE DISKETTE IS EITHER WRITE PROTECTED
;  OR THE OPERATOR HAS SELECTED WRITE PROTECTION FROM THE COMMAND MONITOR)
;
ALIN:    LXI     H,MSG51         ;OUTPUT "ALIGNMENT," TO THE CONSOLE
         CALL    MSG
         CALL    XMTUS   ;TRANSMIT LOAD UNIT/SECTOR TO CONTROLLER
         IN      DATAI   ;GO GET STATUS
         ANI     010H    ;IF IT'S NOT WRITE PROTECTED,
                         ; FIND OUT IF THE OPERATOR HAS
                         ; SELECTED WRITE PROTECTION FROM
                         ; THE COMMAND MONITOR 
         JZ      ALIN7
ALIN1:   LXI     H,MSG53         ;OUTPUT "AUTO?" TO CONSOLE
         CALL    MSG
         CALL    CECHO   ;GO GET KEY-BOARD INPUT
         CPI     'Y'
         JZ      ALIN4   ;GO DO AUTO,IF REQUESTED
         CPI     'N'
         JZ      ALIN2   ;GO DO SELECTED TRACKS,IF REQUESTED
         JMP     LER     ;OOPS WRONG KEY-BOARD INPUT
ALIN2:   CALL    RSTR    ;DO A SEEK TO TRACK 00,THE 
                         ; C.E. DISKETTE IS WRITE PROTECTED 
         CALL    CRLF    ;KEEP IT NEAT 
         CALL    SP1
         MVI     C,'S'   ;OUTPUT "SEEK TO TRACK=" TO CONSOLE
         CALL    CO
         LXI     H,MSG11
         CALL    MSG
         CALL    CONI    ;LET'S LOOK FOR A VALID HEX TRACK NUMBER
         STA     TRKNO   ;STUFF THE KEY-BOARD INPUT INTO TRACK STORAGE
         CPI     04DH    ;TRACK NUMBER TO LARGE?
         JNC     INER1   ;EXIT,IF SO 
ALIN3:   CALL    CLRER   ;GO CLEAR ERROR FLAGS
         CALL    KBINT   ;GO CHECK FOR A KEY-BOARD INTERRUPT
         IN      CCTRL   ;NEW TRACK SEEK REQUESTED?
         ANI     CRRDY
         JZ      OLDTK   ;GO DO OLD TRACK,IF NOT
         IN      CDATA
         ANI     07FH
         CPI     'S'     ;GOT A SEEK REQUEST?
         JZ      ALIN8   ;IF YES,LET'EM CHOOSE 
OLDTK:   CALL    XMTTK   ;TRANSMIT TRACK THE TRACK NUMBER,
                         ; TO THE CONTROLLER
         MVI     A,009H  ;TRANSMIT SEEK COMMAND,TO CONTROLLER
         CALL    XMITW
         JMP     ALIN3   ;STAY ON THE TRACK WITH THE HEAD(S)
                         ; LOADED NO MATTER WHAT HAPPENS 
ALIN4:   CALL    RSTR    ;DO A SEEK TO TRACK 00
         MVI     A,001H  ;SET-UP TRACK 01
         STA     TRKNO
         CALL    ALIN5   ;GO SEEK TO THE TRACK,AND STAY AWHILE 
         CALL    RSTR    ;DO A SEEK TO TRACK 00
         MVI     A,04CH  ;SET-UP TRACK 76
         STA     TRKNO
         CALL    ALIN5   ;NOW HANG-OUT AT TRACK 76 FOR AWHILE 
         JMP     ALIN4   ;NOW LET'S DO IT ALL OVER AGAIN 
                         ; IS'NT THIS FUN          
ALIN5:   MVI     A,007H  ;SET-UP THE TIMER ITERATIONS COUNT
ALIN6:   STA     TIMCT
         CALL    KBINT   ;GO CHECK FOR A KEY-BOARD INTERRUPT
         CALL    XMTUS   ;TRANSMIT LOAD UNIT/SECTOR,
                         ; TO CONTROLLER
         CALL    XMTTK   ;TRANSMIT TRACK NUMBER,
                         ; TO CONTROLLER
         MVI     A,009H  ;TRANSMIT SEEK TRACK COMMAND,
                         ; TO CONTROLLER
         CALL    XMITW
         CALL    CLRER   ;TRANSMIT CLEAR ERROR FLAGS,
                         ; TO CONTROLLER
         CALL    TIME3   ;GO KILL SOME TIME 
         LDA     TIMCT   ;GET THE TIMER ITERATIONS COUNTER
         DCR     A       ;DECREMENT IT 
         CPI     000H    ;DONE?
         RZ              ;RETURN,IF SO 
         JMP     ALIN6   ;NOPE STAY A LITTLE WHILE LONGER 
ALIN7:   CALL    WPRT    ;GO CHECK FOR OPERATOR REQUESTED
                         ; WRITE PROTECT
         JNZ     ALIN1   ;GO DO THE ALIGNMENT ROUTINE,IF SO 
         CALL    SP2
         LXI     H,MSG52         ;OUTPUT "NO WRITE PROTECT" TO CONSOLE
         CALL    MSG
         CALL    WPMSG
         JMP     MNTR    ;EXIT TO THE MONITOR,UNTIL THINGS
                         ; ARE SQUARED AWAY 
ALIN8:   CALL    TIME1   ;MAKE A DELAY
         JMP     ALIN2
;
;
;
;
;
;
;ROUTINE TO DELAY TIME (PAUSE) AS REQUIRED (DELAY CONSTANT IN H&L REGS.)
;
DLY:     DCX     H
         MOV     A,L
         ORA     H
         JNZ     DLY
         RET
;
;
;
XMTUS:   LDA     SECNO   ;TRANSMIT UNIT/SECTOR TO CONTROLLER
         MOV     B,A
         LDA     UNIT
         ORA     B
         OUT     DATAO
         MVI     A,21H
         CALL    XMIT
         CALL    UNCHK   ;CHECK UNIT CODE BITS FOR MATCH
                         ; TO SELECTED UNIT
         RET
;
;
;
XMTTK:   LDA     TRKNO   ;TRANSMIT TRACK ADDRESS TO CONTROLLER
         OUT     DATAO
         MVI     A,11H
         JMP     XMIT
;
;
;
XMIT:    OUT     CNTRL
         XRA     A       ;OUTPUT A "DUMMY" EXAMINE STATUS TO
                         ; CONTROLLER TO CLEAR DATA OUT INTERFACE
         OUT     CNTRL
         CALL    DCHK    ;GO TEST DRIVE FAIL BIT
         RET

;
;
;
XMT4X:   OUT     CNTRL
         IN      DATAI
         MOV     B,A
         XRA     A
         OUT     CNTRL
         RET
;
;
;
XMITW:   CALL    XMIT
         LDA     TIME    ;GO GET TIMER CONTROL COMMAND
         ADI     000H
         JNZ     XMITT   ;IF TIMER COMMAND IS SET,GO DO
                         ; A 2.5 SECOND DELAY AND 
                         ; DON'T WAIT FOR BUSY STATUS TO
                         ; GO FALSE 
         MVI     B,003H  ;SET-UP TIMER CONSTANTS (5 SECONDS)
         LXI     H,0FFFFH
BUSYT:   DCX     H       ;START COUNTING DOWN
         MOV     A,L
         ORA     H
         JNZ     BUSY    ;TIMED OUT?
         DCR     B
         JNZ     BUSY
         LXI     H,MSG56         ;OH  OH  "RUN-AWAY" BUSY   
         CALL    MSG
         CALL    SCLR    ;DO A SYSTEM CLEAR
         JMP     MNTR    ;THIS CONTROLLER IS IN SO MUCH TROUBLE,
                         ; THAT WE MIGHT AS WELL GO BACK TO
                         ; THE COMMAND MONITOR 
BUSY:    IN      DATAI   ;TEST BUSY STATUS FROM CONTROLLER
         RAR
         JC      BUSYT
         RET
;
;
;
XMITT:   CALL    TIME1   ;DELAY FOR 2.5 SECONDS.
         RET
;
;
;
;DRIVE READY TEST
;
DCHK:    IN      DATAI   ;GET STATUS FROM CONTROLLER
         ANI     020H    ;TEST DRIVE FAIL BIT
         RZ
         CALL    CRLF
         CALL    SP1
         LXI     H,MSG31         ;OUTPUT "UNIT=" TO CONSOLE
         CALL    MSG
         CALL    AUNIT   ;FORM UNIT STORAGE INTO PROPER
                         ; POSITION FOR CONSOLE OUTPUT
         CALL    BYTEO
         CALL    SP1
         LXI     H,MSG34         ;OUTPUT "DRIVE FAIL" TO CONSOLE
         CALL    MSG
         RET
;
;
;
;SUBROUTINE TO CHECK UNIT CODE BITS FOR MATCH TO SELECTED UNIT
;
UNCHK:   LDA     UNIT    ;GET THE UNIT NUMBER 
         RLC
         RLC
         RLC
         MOV     B,A     ;MATCH IT TO BIT POSITION FOR UNIT CODE BITS
         IN      DATAI   ;GET'EM 
         ANI     006H    ;MASK'EM 
         CMP     B       ;GOT A MATCH?
         RZ
         CALL    SP1     ;OOPS  NO MATCH   
         LXI     H,MSG31         ;OUTPUT "UNIT=" TO CONSOLE
         CALL    MSG
         CALL    AUNIT
         CALL    BYTEO   ;SHOW'EM THE UNIT NUMBER 
         CALL    SP1     ;KEEP IT NEAT
         LXI     H,MSG10         ;OUTPUT "UNIT CODE FAIL" TO CONSOLE
         CALL    MSG
         RET
;
;
;
;WRITE PROTECT TEST SUBROUTINE
;
WPCHK:   IN      DATAI   ;GET STATUS FROM CONTROLLER
         ANI     010H    ;TEST WRITE PROTECT BIT
         RZ
         CALL    SP1
         LXI     H,MSG31         ;OUTPUT "UNIT=" TO CONSOLE
         CALL    MSG
         CALL    AUNIT   ;FORM UNIT NUMBER FROM STORAGE,
                         ; INTO ACTUAL UNIT NUMBER
         CALL    BYTEO
WPMSG:   CALL    SP1
         LXI     H,MSG35         ;OUTPUT "WRITE PROTECT" TO CONSOLE
         CALL    MSG
         CALL    CRLF
         RET
;
;
;
;ROUTINE TO ALLOW THE OPERATOR TO SET "WRITE PROTECT"
;
PROT:    LXI     H,MSG35         ;OUTPUT "WRITE PROTECT" TO CONSOLE
         CALL    MSG
         LXI     H,YNMSG         ;OUTPUT "(Y,N)" TO CONSOLE
         CALL    MSG
         MVI     C,'?'
         CALL    CO
         CALL    SP1
         CALL    CECHO   ;GO GET KEY-BOARD INPUT
         CPI     'Y'     ;YES?
         JZ      PROT1
         CPI     'N'     ;NO?
         JZ      PROT2
         JMP     LER     ;OOPS WRONG ENTRY 
PROT1:   MVI     A,001H  ;YES WRITE PROTECT THIS DISK 
         JMP     PROT3
PROT2:   XRA     A       ;WERE GOING TO WRITE ON IT 
PROT3:   STA     WPROT
         JMP     MNTR
;
;
;
;
;SUBROUTINE TO TEST FOR OPERATOR REQUESTED WRITE PROTECT
;
WPRT:    LDA     WPROT   ;GO GET STATE OF WRITE PROTECT COMMAND
         ANI     001H    ;GO FIND OUT AT THE RETURN 
         RET
;
;
;
;STORAGE FOR VARIABLES REQUIRED FOR FLEXIBLE DISKETTE DRIVE/CONTROLLER
;
UNIT:    DS      1       ;STORAGE FOR DISKETTE DRIVE "UNIT" NUMBER
TRKNO:   DS      1       ;STORAGE FOR TRACK/CYLINDER ADDRESS
LSN:     DS      1       ;STORAGE FOR LOGICAL SECTOR NUMBER
SECNO:   DS      1       ;STORAGE FOR PHYSICAL SECTOR NUMBER
DBUF:    DS      1       ;STORAGE FOR PATTERN SELECTED DATA BYTE
BLOOP:   DS      1       ;STORAGE FOR "LOOP" COMMAND
RDCRC:   DS      1       ;STORAGE FOR "READ CRC" COMMAND
TRIES:   DS      1       ;STORAGE FOR "ERROR RETRIES"
SAVE:    DS      1       ;STORAGE FOR "SAVED STATUS"
RETRY:   DS      1       ;STORAGE FOR "READ ERROR RETRY" COUNTER
SHDBE:   DS      1       ;STORAGE FOR "S/B" DATA FROM WRITE BUFFER
WAS:     DS      1       ;STORAGE FOR "WAS" DATA BYTE FROM READ BUFFER
DCTR:    DS      2       ;STORAGE FOR "BYTE IN ERROR" DATA COUNTER
ECTR:    DS      2       ;STORAGE FOR "ERROR" COUNTER
PCTR:    DS      2       ;STORAGE FOR "PASS" COUNTER
RCTR:    DS      2       ;STORAGE FOR "RANDOM SEEK ITERATIONS" COUNTER
RPNT:    DS      2       ;STORAGE FOR RANDOM NUMBER TABLE POINTER
DELET:   DS      1       ;STORAGE FOR "DELETED DATA MARK" COMMAND
NEWTK:   DS      1       ;STORAGE FOR TRACK NUMBER,IF "LOOP" COMMAND
                         ; IS ENABLED ON "SEEK TO TRACK" TEST
TIME:    DS      1       ;STORAGE FOR "TIMER CONTROL" COMMAND
TIMCT:   DS      1       ;STORAGE FOR TIMER ITERATIONS COUNT
WPROT:   DS      1       ;STORAGE FOR "WRITE PROTECT" COMMAND
VFYCM:   DS      1       ;STORAGE FOR "VERIFY" (READ CRC) COMMAND
;
;
;
;STORAGE FOR WRITE,READ,RANDOM TRACK,AND STACK BUFFERS
;
WBUF     DS     128        ;STORAGE FOR 128 BYTE WRITE DATA BUFFER
RBUF     DS     128        ;STORAGE FOR 128 BYTE READ DATA BUFFER
RANTK    DS     256        ;STORAGE FOR 256 RANDOM TRACK TABLE
 	 DS	512
STACK	 EQU	$	   ;STORAGE FOR 512 BYTE STACK
;
;
;
         END
