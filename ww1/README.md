# Wartime Films Project - World War I

### [App](https://github.com/usnationalarchives/wartime-films-project/blob/master/ww1/app)
The metadata posted and enriched on GitHub will feed into a U.S. National Archives World War I app that will highlight NARA wartime films content, improving content discoverability and relevance for local audiences. The primary purpose of the app is to highlight NARA World War I content specific to the Wartime Films digitization and preservation effort, as well as World War I content contributed from other institutions and individuals, increasing the relevance and discovery of this content for local communities.  

We dedicate the codebase of this software to the public domain in the United States, in addition to waiving copyright and related rights in the work worldwide with a CC0 license. The codebase will be extended as needed. It is published here on NARA’s GitHub for others to further modify and repurpose (see [Terms of Use](https://github.com/usnationalarchives/Wartime-Films-Project/blob/master/LICENSE)).

### [Metadata](https://github.com/usnationalarchives/wartime-films-project/blob/master/ww1/metadata)

The photographic files contained in the series ["American Unofficial Collection of World War I Photographs, 1917-1918"](https://catalog.archives.gov/id/533461) are a mesmerizing slice of American life during the years leading up to and after the U.S. involvement in World War I. The collection is richly described and in many instances contains geographic location information contained within titles or descriptions. Subjects include bonds, savings stamps and war loan drives, public gatherings, peace demonstrations and parades. Also documented are activities of libraries, hospitals, first aid stations, training camps and forts, and groups such as African-Americans in the military, Women's Suffrage activities, and much more. 

### National Archives Catalog API 

While we’re posting code and metadata in this repository, you can also access the data directly from the [National Archives Catalog API](https://github.com/usnationalarchives/Catalog-API).

The National Archives Identifier (NAID) for the series ["American Unofficial Collection of World War I Photographs, 1917-1918"](https://catalog.archives.gov/id/533461) is `533461`. The metadata for the series itself can be queried with:

- [`https://catalog.archives.gov/api/v1/?naIds=533461`](https://catalog.archives.gov/api/v1/?naIds=533461)

To query for the descriptive metadata of the series' photographic files, use:

- [`https://catalog.archives.gov/api/v1/?description.fileUnit.parentSeries.naId=533461&type=description`](https://catalog.archives.gov/api/v1/?description.fileUnit.parentSeries.naId=533461&type=description)

To get all of the descriptive metadata, including tags/transcriptions/comments, for the items (the individual photographs) for all of the files in the series, query:

- [`https://catalog.archives.gov/api/v1/?description.item.parentFileUnit.parentSeries.naId=533461&type=description`](https://catalog.archives.gov/api/v1/?description.item.parentFileUnit.parentSeries.naId=533461&type=description)

These descriptions include the URL path and metadata for the digital media (called "objects" in our API) attached to each item. You can also get just objects by changing the `&type=` parameter's value from `description` to `object` in the above query URL.

If you wanted to look at the photos in just one particular file, you would determine its NAID, and then use that to form a query like this example for the file unit "[American Red Cross - A thru D](https://catalog.archives.gov/id/20797218)", which has the NAID `20797218`:

- [`https://catalog.archives.gov/api/v1/?description.item.parentFileUnit.naId=20797218`](https://catalog.archives.gov/api/v1/?description.item.parentFileUnit.naId=20797218)

For any of the above sample queries, you can append a keyword parameter (`&q=`) and a number of other fielded search parameters to restrict your search results. For more information on the available functions and format for NARA's catalog API to perform more complex queries, see [the documentation in its repo](https://github.com/usnationalarchives/Catalog-API).
