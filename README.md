# FantasyGroundsPackager
CLI tool for packaging Fantasy Grounds extensions

# Usage
Currently this supports targeting a directory containing a Fantasy Grounds extension.

Simply run the program via command line and pass the directory of your extension. The packager will find the `extension.xml` within and create a `.ext` file using the name of the directory specified.
The package will contain all the dependencies specified within the `extension.xml` file plus any `.md` files at the root directory specified.

ex. `.\FantasyGroundsPackager.exe C:\Users\User\source\repos\FG-Attack-On-Hit-Effects`

The output artifact will be in the root directory specified, so in our example `C:\Users\User\source\repos\FG-Attack-On-Hit-Effects\FG-Attack-On-Hit-Effects.ext`
