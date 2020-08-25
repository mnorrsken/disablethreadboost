# disablethreadboost
Disables Dynamic Thread Boosting, sets affinity to all physical cores minus one, and optionally sets low priorito for the MS FS2020 process.

-a <affinity>
sets affinity manually (decimal number) or -a 0 disables affinity modification

-n
don’t disable thread priority boost

-p
also set process to Low priority 

Feel free to do whatever you want with the code.
