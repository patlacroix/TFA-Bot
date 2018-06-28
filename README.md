# TFA-Bot
### Factom Node Monitor

#### Prerequisites
* A well connected server, with Docker for running your Bot on.
* A private Discord Server account.
* Factom nodes to monitor!  Make sure port 8088 is open to your Bot server.

----

#### Getting started

1. Copy this Google spreadsheet, rename it, and fill in every sheet tab.  Follow the instructions on the "Settings" tab.
https://docs.google.com/spreadsheets/d/19SLbCQLFKpkSaZ88SAmN_Mg8L8M-TkiB67TJD67lNQA/edit?usp=sharing

2. Get a read-only share URL for your spreadsheet. (Dont share it with anyone, but you can invite your members for full read/write access)

3. On your server (using your own URL):

```
docker build -t tfa-bot https://git.factoid.org/TFA/TFA-Bot.git
docker run --rm -d -e "BOTURL=https://docs.google.com/spreadsheets/d/123456789123456789123456789123456789/edit?usp=sharing" --name bot tfa-bot
```

##### Optional SIP Server, for Phone Call alerts

TFA-Bot uses [SIPp](https://github.com/SIPp/sipp) to make phone calls via a SIP VOIP gateway.
The supplied dialplan.xml may work for you. It calls any given number until it's answered (or times out).
Some services may require a custom dialplan or "scenario", which you can test using the sipp bash command.
[Alternative scenario scrips here](https://github.com/saghul/sipp-scenarios)

Fill in your SIP settings on the spreadsheet.  You can optionally provide the password to docker using -e "SIP-PASSWORD=...."

----

#### Operation

Type 'Help' in any discord channel where the Bot is present.  You can add the Bot to all your Discord channels so that it reacts to your commands wherever you are.
The Bot will monitor Factom's #operators-alerts channel.  Anything posted there, will be copied to your own Bot Alerts channel.

Type 'nodes' to see the current state.

