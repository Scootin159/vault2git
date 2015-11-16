# vault2git
A SourceGear Vault to Git conversion application

This project is based off of https://github.com/slamotte/vault2git, but was ported to C# and additional features were added.

This works by brute-force running through all the check-ins for a particualr Vault repository directory, applying all check-ins and labels (as 'tags') along the way.  This technique works, but is relatively slow.  It could take hours or days to convert a large repository, but hopefully you don't need to run this too many times.
