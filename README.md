# monitoring
This repository exists to contain instructions + code necessary to setup a server with kibana, primarily for logging.
At some point this could be expanded to time series as well.

## Steps
### Prerequisites
This README assumes you are running commands on an Ubuntu system with docker installed.

### System Configuration
The VPS will need at least 4GB of RAM.

Add this to the `/etc/sysctl.conf` file and reboot: `vm.max_map_count=262144`
Without this elasticsearch won't start. Updating the file makes sure it persists across reboots.
See: https://stackoverflow.com/a/51448773/1663648

### Install
Copy the `docker-compose.yml` file into the VPS and then run it by running: `docker-compose up -d`
Run the command in the directory that contains the `docker-compose.yml`. 

### Kibana Configuration
#### Dark Theme
Kibana has a dark theme! In the navigation go to `Stack Management` -> `Advanced Settings` and search for "theme".

#### Logs
Add the index pattern in `Stack Management` -> `Index Patterns`.
The logs just come directly out of elasticsearch given an index, so for logging image this.
`PUT http://localhost:9200/tix-factory/logs/1`
```json
{
    "message": "hello, world!",
    "labels": {},
    "host": {
        "name": "host name heree"
    },
    "log": {
        "name": "TFAAS1.TixFactory.ApplicationAuthorization.Service",
        "level": "Warning"
    },
    "@timestamp": "2020-10-04T13:55:15.887Z"
}
```

Without the `labels` field it doesn't seem to work? idk