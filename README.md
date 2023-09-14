# audio-guestbook-net

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=svartis_audio-guestbook-net&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=svartis_audio-guestbook-net)

## Inspired by:

https://www.youtube.com/watch?v=dI6ielrP1SE

https://github.com/playfultechnology/audio-guestbook

## Why this setup:
In the original solution a Teensy 4.0 with an Audio Board is used as hardware.
And since I don't want to buy any extra hardware for a 1 time time project I decided to create an solution that runs on Raspberry Pi 4.

Note: The Raspberry Pi 4 also require a USB soundcard to be able to record audio.

## Limitations:
Due to NAudio framework that is used for playing and recording wav files only working on windows the Raspberry Pi 4 is running Windows 11.

https://www.worproject.com/

## Diagram:
![](https://raw.githubusercontent.com/svartis/audio-guestbook-net/blob/main/diagram.png)