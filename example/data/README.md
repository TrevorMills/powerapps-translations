# Sample Data

Here is sample data that can be used if you want to try and stand up the sample app in [Translation Demo.msapp](./../Translation%20Demo.msapp).

## Important Details

### Manual Creation of Lists

Unfortunately, you cannot simply create lists in SharePoint from these CSV files.  If you do that process, it will give the columns logical names like `field_1`, `field_2`, etc.  The logical name is different than the column title.  However, the TranslationComponent needs them to be the same.  So, you must create the lists manually in SharePoint.  When you do that, the logical names will be the same as the column names.

### Required Columns

#### Translations List

* `Title` will contain the original language version of the string
* `Context` would provide context to strings. Make the default value for this just the string "default".  Context provides a mechanism if the same string would be translated differently depending on the context.  It's not used much, but the Translation Component requires the column be there and filled in with the string "default"
* `Long` would be a Multiline Text field and gives a way to have longer strings or strings with HTML be translated as well

#### Choices List

* `Title` will contain the original language version of the choice
* `Group` will contain a string that groups choices together. Choices that would go into the same dropdown should have the same value for Group

### LanguageSchema

The LanguageSchema property of the TranslationComponent is how you specify which columns it should look to for each of your languages.

A sample in the TranslationComponent looks like this:

```json
Table(
    {
        Code: "en",
        Column: "Title",
        LongColumn: "Long",
        ChoiceColumn: "Title"
    },
    {
        Code: "fr",
        Column: "TitleFR",
        LongColumn: "LongFR",
        ChoiceColumn: "TitleFR"
    }
)
```

The `Column`, `LongColumn` and `ChoiceColumn` refer to columns in your Translations and Choices list.  You will likely need to set up your own schema here.  Let's say you are translating Spanish as well as French.  Then you would add the following record to your `LanguageSchema` property:

```json
{
    Code: "es",
    Column: "TitleES"
    LongColumn: "LongES",
    ChoiceColumn: "TitleES"
}
```

You'll then add `TitleES` and `LongES` to your `Translations` List, and add `TitleES` to your `Choices` list.

There's one more critical step that is needed.  The `TranslationComponent` component has default hardcoded values for `Translations` and for `Choices`.  Power Apps uses those hardcoded values to whitelist the columns it allows through in tables passed in to it.  You must update these default values to include your new columns.

To update those default values, edit the `TranslationComponent` itself.

#### Set `Translations` to:

```json
Table(
    {
        ID: 1,
        Title: "Sample text",
        TitleFR: "Exemple de texte",
        TitleES: "",
        Context: "default",
        Long: "This is longer text",
        LongFR: "C'est un texte plus long",
        LongES: ""
    }
)
```

#### Set `Choices` to:

```json
Table(
    {
        ID: 1,
        Title: "English Title",
        TitleFR: "Title Français",
        TitleES: "",
        Group: "MyGroup",
        IsOther: false
    }
)
```

> **Quirk** - after saving the values in the `TranslationComponent`, it may be necessary to save and refresh the edit app page and then update your `T` component to point to the correct data sources for Translations and Choices again.  This is a Power Apps quirk we noticed while developing this solution.
