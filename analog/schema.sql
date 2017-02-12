CREATE TABLE `entries` ( 
    `datetime` TEXT NOT NULL, 
    `ip` TEXT, 
    `status` INTEGER NOT NULL, 
    `method` TEXT NOT NULL, 
    `url` TEXT NOT NULL, 
    `query` TEXT, 
    `useragent` TEXT, 
    `bytesout` INTEGER NOT NULL, 
    `bytesin` INTEGER NOT NULL 
)