# TFA-Bot
### Factom Node Monitor

#### Prerequisites
* A server, with Docker for running your Bot on.
* A Discord Server account.
* Factom nodes to monitor!

----

#### Getting started

1. Copy this Google spreadsheet, and fill in every sheet tab.  Follow the instructions on the "Settings" tab.
https://docs.google.com/spreadsheets/d/19SLbCQLFKpkSaZ88SAmN_Mg8L8M-TkiB67TJD67lNQA/edit?usp=sharing

2. Get a read-only share URL for your spreadsheet. (Dont share it with anyone, but you can invite your members for full read/write access)

3. On your server:

```
docker build -t tfa-bot https://git.factoid.org/TFA/TFA-Bot.git
docker run --rm -d -e "BOTURL=https://docs.google.com/spreadsheets/d/123456789123456789123456789123456789/edit?usp=sharing" --name bot tfa-bot
```

#### Optional SIP Server, for Phone Call alerts

TFA-Bot uses [SIPp](https://github.com/SIPp/sipp) to make phone calls via a SIP VOIP gateway.
The supplied dialplan.xml may work for you. It calls any given number until it's answered (or times out).
Some services may require a custom dialplan, which you can test using the sipp bash command.

Fill in your SIP settings on the spreadsheet.  You can optionally provide the password to docker using -e "SIP-PASSWORD=...."