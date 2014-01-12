RESTORE DATABASE TeamMashup
FROM  DISK = N'D:\Backups\<<filename>>.bak'
WITH  FILE = 1,  NOUNLOAD,  STATS = 10