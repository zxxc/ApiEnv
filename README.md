# EnvValue
Visual Studio extension to change environment variable `EnvTest`
It will adds combobox with possible configurations for tests

For the tests we can create files with configurations:

- TestSettings.json
- TestSettings.local.json
- TestSettings.local.vm1.json
- TestSettings.staging.json
- TestSettings.dev.json

This extension will show list:
- `<empty>`
- local
- local.vm1
- staging
- dev

By selecting line we will change environment variable `EnvTest` to selected value. And kill test settings executors like mstest, xunit, vstest