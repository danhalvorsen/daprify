#!/bin/bash

get_crt_content() {
    local crt_path="$1"

    if [ ! -f "$crt_path" ]; then
        echo "Certificate file not found: $crt_path"
        exit 1
    fi

    cat "$crt_path"
}

check_and_create_env_file_exists() {
    local env_file_path="$1"
    local env_dir_path=$(dirname "$env_file_path")

    if [ ! -f "$env_file_path" ]; then
        mkdir -p "$env_dir_path" && touch "$env_file_path"
        return 1
    fi
    return 0
}

verify_var_exists() {
    local env_var_name="$1"

    if grep -q "^$env_var_name=" "$ENV_FILE_PATH"; then
        return 0
    else
        return 1
    fi
}

delete_env_var() {
    local env_var_name="$1"
    local old_variable="$2"
    local new_variable="$3"

    if grep -q "^$env_var_name=" "$ENV_FILE_PATH"; then
        awk -v var="$env_var_name" '$0 ~ "^"var"=" {p=1} /^$/ {if(p) {p=0; next}} !p' "$ENV_FILE_PATH" > temp.env && mv temp.env "$ENV_FILE_PATH"
    fi
}

create_env_var() {
    local new_variable="$1"
    local append_new_line="$2"

    if [ "$append_new_line" = true ]; then
        echo  >> "$ENV_FILE_PATH"
    fi
    echo "$new_variable" >> "$ENV_FILE_PATH"
}

base_dir="$1"

CA_CRT_PATH="$base_dir/ca.crt"
ISSUER_CRT_PATH="$base_dir/issuer.crt"
ISSUER_KEY_PATH="$base_dir/issuer.key"
ENV_FILE_PATH="$base_dir/../Env/Dapr.env"

CA_CONTENT=$(get_crt_content "$CA_CRT_PATH")
ISSUER_CRT_CONTENT=$(get_crt_content "$ISSUER_CRT_PATH")
ISSUER_KEY_CONTENT=$(get_crt_content "$ISSUER_KEY_PATH")

NEW_TRUST_ANCHORS="DAPR_TRUST_ANCHORS=\"$CA_CONTENT\""
NEW_CERT_CHAIN="DAPR_CERT_CHAIN=\"$ISSUER_CRT_CONTENT\""
NEW_CERT_KEY="DAPR_CERT_KEY=\"$ISSUER_KEY_CONTENT\""

check_and_create_env_file_exists "$ENV_FILE_PATH"

if verify_var_exists "DAPR_TRUST_ANCHORS"; then
    delete_env_var "DAPR_TRUST_ANCHORS" "$NEW_TRUST_ANCHORS" "$NEW_TRUST_ANCHORS"
fi

if verify_var_exists "DAPR_CERT_CHAIN"; then
    delete_env_var "DAPR_CERT_CHAIN" "$NEW_CERT_CHAIN" "$NEW_CERT_CHAIN"
fi

if verify_var_exists "DAPR_CERT_KEY"; then
    delete_env_var "DAPR_CERT_KEY" "$NEW_CERT_KEY" "$NEW_CERT_KEY"
fi

create_env_var "$NEW_TRUST_ANCHORS" false
create_env_var "$NEW_CERT_CHAIN" true
create_env_var "$NEW_CERT_KEY" true