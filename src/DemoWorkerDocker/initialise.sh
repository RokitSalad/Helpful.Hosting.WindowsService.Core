#!/bin/sh

systemctl daemon-reload
systemctl status demoworkerdocker
systemctl start demoworkerdocker.service
