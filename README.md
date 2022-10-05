# vault2git
A SourceGear Vault to Git conversion application

This project is based off of https://github.com/slamotte/vault2git, but was ported to C# and additional features were added.

This works by brute-force running through all the check-ins for a particualr Vault repository directory, applying all check-ins and labels (as 'tags') along the way.  This technique works, but is relatively slow.  It could take hours or days to convert a large repository, but hopefully you don't need to run this too many times.

To run, simply call the program vault2git.exe with the appropriate command-line parameters.  Once your process is complete, you can push all your changes to the appropriate GitHub repository.  Use the command "git push --tags" to push your tags to the GitHub repository.

You can either start with an empty (or non-existing) working directory, or by using the "--start" parameter, you can append to an existing repository starting with the specified vault history version.

Command-line options:
  --vault_path      (Default: C:\Program Files (x86)\SourceGear\Vault
                    Client\vault.exe) The path to the SourceGear Vault command
                    line utility (vault.exe)
  -s, --host        Required. Vault server hostname
  --ssl             (Default: False) Use SSL for the vault connection
  -u, --username    Required. Vault username
  -p, --password    Required. Vault Password
  -r, --repo        Required. The Vault repository name
  -i, --input       (Default: $) The Vault respository path ($/source/path)
  --start           (Default: none) The Vault date to start converting at
  --end             (Default: none) The Vault date to end converting at
  --git_path        (Default: git.exe) The path to the Git command-line tool
  --authors         (Default: authors.xml) The path to an XML mapping of Vault
                    users to Git users
  -o, --output      Required. The Git output working directory
  --logfile         (Default: log.txt) The log file path
  --help            Display this help screen.

  What is converted:
  - File check-ins
  - File deletions
  - Labels
  - The date of the original action
  - The user of the original action (note authors.xml)

To properly map authors, make sure to have a file named "authors.xml" within the root directory.  GitHub will automatically (and retroactively where required) match users based on the email address.

Here is a sample authors.xml file:
```
<authors>
    <author vaultname="John" name="John Smith" email="john@exmple.com" />
    <author vaultname="Jane" name="Jane Smith" email="jane@exmple.com" />
</authors>
```
