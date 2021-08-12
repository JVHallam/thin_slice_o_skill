#!/bin/sh

connection_string=$(terraform output connection_string | tr -d '"')
export connection_string
