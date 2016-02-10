# Kexjobb

When cloning this repository, use the following command: 
git clone --recursive https://github.com/Sandsten/Kexjobb.git

"--recursive" will make sure the submodule (UnitySteer) is downloaded as well.
If the submodule didn't exist in the master branch i.e only on the develop branch
the submodule folder will be empty.

Then you will have to use the following command while on the develop branch:
git submodule update --init --recursive


