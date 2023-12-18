#!/bin/bash

base_dir="$1"

# working_dir="$base_dir/Dapr/Certs"

# Create directory if it doesn't exist
# mkdir -p "$working_dir"

ca_crt="$base_dir/ca.crt"
ca_key="$base_dir/ca.key"
issuer_crt="$base_dir/issuer.crt"
issuer_key="$base_dir/issuer.key"

create_certificate() {
  local profile=$1
  local crt_file=$2
  local key_file=$3
  local ca_crt=$4
  local ca_key=$5

  step certificate create cluster.local "$crt_file" "$key_file" --ca "$ca_crt" --ca-key "$ca_key" --profile "$profile" --no-password --insecure
}

create_certificate "root-ca" "$ca_crt" "$ca_key" "" ""
create_certificate "intermediate-ca" "$issuer_crt" "$issuer_key" "$ca_crt" "$ca_key"

# Remove the CA key as it is not needed for Dapr
rm "$ca_key"
