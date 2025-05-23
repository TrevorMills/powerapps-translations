# TL;DR

Add the `TranslationComponent` to your app and rename it to `T`.  Provide the proper input parameters, and then use the output parameters throughout your app:

* `T.__("My String")` - use this _everywhere_ you hvae a string that needs to be translated
* For every dropdown:
  * For a dropdown controlling a Lookup field
    * Set `Items` to `T.__Choices("MyGroup")`
    * Set `DefaultSelectedItems` to `Filter( T.__Choices("MyGroup"), Id = ThisItem.MyGroup.Id)`
  * For a dropdown controlling a Choice field
    * Set `Items` to `T.__Choices("MyGroup")`
    * Set `DefaultSelectedItems` to `Filter( T.__Choices("MyGroup"), Title = ThisItem.MyGroup.Value)`
    * Set the `Update` property of the parent DataCard to `{Value: DataCardValue1.Selected.Title}`
* For places you want to show the value of a choice field statically ( i.e. in a Vertical Gallery )
  * For a Lookup field
    * Set `Text` to `LookUp( T.__Choices("MyGroup"), Id = ThisItem.MyGroup.Id).Value`
  * For a Choices field
    * Set `Text` to `LookUp( T.__Choices("MyGroup"), Title = ThisItem.MyGroup.Value).Value`


# Adding TranslationComponent

The `TranslationComponent` is a bit unlike other components you may be used to.  It doesn't provide anything visual.  In fact, it is a 0x0 component with nothing in it.

What it provides are two output functions helpful for a developer building a multilingual app.

* `__` is a function that helps in the translations of strings
* `__Choices` is a function that helps with the translations of Choice columns.

You can include the `TranslationComponent` a couple of ways within your Solution.  If you're going to have lots of apps, we recommend adding it into a `Component Library`.  If your solution is just going to have a single app, you could add it as a `Custom Component` in there.

## Add as Component Library

In your solution, you're going to add a new `Component Library`.

* Click on `+ New` > `More` > `Component Library`
* That should create a blank component library with a single component called `Component1`
* Copy the contents of [TranslationComponent.yaml](TranslationComponent.yaml) to your clipboard, then under the three dots of `Component1`, choose `Paste` > `Paste Code`.  That should create a `TranslationComponent` with the settings from this repo.  You can delete `Component1`.
* Save your `Component Library`.

## Add as Custom Component

If you just want to add the `TranslationComponent` as a Custom Component in a single app, you can do that too.  Edit your app, then:

* Go to the `Tree View` and then the `Components` tab
* Click `+ New Component`.  That will create `Component1`
* Copy the contents of [TranslationComponent.yaml](TranslationComponent.yaml) to your clipboard, then under the three dots of `Component1`, choose `Paste` > `Paste Code`.  That should create a `TranslationComponent` with the settings from this repo.  You can delete `Component1`.

# Using TranslationComponent

Now that the TranslationComponent is added to your solution, it's time to learn how to use it.

Like other components, you'll add `TranslationComponent` to your app.  It is a 0x0 component with nothing visual in it, so you won't see it within your app.

Add the component as you would any other.  It can be added to any screen within your app and its functions will be available on all screens.

> Because you will be typing it a lot, we recommend renaming the component to something short for brevity.  Try something simple like just `T`

## Input Parameters

The `TranslationComponent` accepts four input parameters:

### Language (text)

Should be the language code of whatever the current language of the app is.  The default is "en", but you will likely set this to a variable that is controlled by some language switcher component in your app.  You can use the two-letter form for languages ( i.e. "en" ) or the longer form ( i.e. "en-US" ).  All that is required is that the language codes match what is in your `LanguageSchema`

### LanguageSchema (Table)

This defines what the component needs to know about the different languages in your app.  What you provide is a Table with a Record for each language.  Each Record must have the following properties:
  * `Code` - the code for the language. Can be anything, but must match up with whatever language you are passing into the `Language` input of the component
  * `Column` - this is the column in the `Translations` input table that contains the string in this language.  For your default language, it will likely be "Title"
  * `LongColumn` - the translation component allows for a longer HTML column to contain longer or more sophisticated strings that you want translated.  This property should be the name of that column in the `Translations` table for this language.
  * `ChoiceColumn` - the name of the column in the `Choices` table that contains the label for the choice in this language

 **CRITICALLY IMPORTANT** All columns that are referenced in your language schema **must** also show up in the default value for the Translations input parameter ( cares about `Column` and `LongColumn` columns ) or the Choices input parameter ( cares about `ChoiceColumn` columns ).  This is because Power Apps uses that default value to define the schema for these tables, and therefore what columns it will allow.  Changing the default values means updating the your custom component ( or library component if you went that direction ).  Translations will not show up if you don't do this critical step.

 It's sufficient to just put a new column in once.  So, if you're adding a reference to a Column called "SpanishTitle", just add `SpanishTitle: ""` to the first record in the default Translations table of the `TranslationComponent`.

![Language Schema](language-schema.png)

### Translations (Table)

Pass in the SharePoint list ( or Dataverse table ) that contains your translated strings.  The columns for secondary languages are arbitrary, but must refer back to the columns referenced in the `LanguageSchema` input.  At the least, this table should contain these columns:
  * `Title` - this will be the string in your default language
  * `Context` - we'll explain a bit more about context in a moment.  This text column would contain the context for the translated string

### Choices (Table)

The SharePoint list ( or Dataverse table ) that contains translations for LookUp or Choices columns within your app.  The columns containing the translations are arbitrary but need to match the columns referenced in `LanguageSchema`.  Beyond that, the colums that are required in this table are:
  * `Group` - this text field is how choices for a single field are gropued together
  * `IsOther` - choices are sorted alphabetically ( within the current language ), with any choices marked as true in this Yes/No field being sorted to the end

## Output Parameters

The `TranslationComponent` provides two functions as output parameters

### __(String) returns Text

As a developer, while you are building out your app, you'll want to get strings into your app ( labels, instructions, properties of other components ), but you won't necessarily have the translations set up yet.  The `TranslationComponent` allows you to just put in the default language string and worry about translations later.  If you've named the `TranslationComponent` you've added to your app `T` ( for brevity, as we recommended ), then every single place you need a string to appear, you use one of these forms of the `T.__()` formula:

#### No context supplied ( Context "default" )
* `T.__("My String")` - simplest form.  Will return "My String" until a translation exists in the `Translations` table.
* `T.__("my_key")` - the idea with keys is to have them be a placeholder for longer or HTML text that get stored in the `LongColumn` column specified in your `LanguageSchema`.  Until that is in place, it will return "my_key".

#### Context Supplied

If you do not supply the second argument, the `TranslationComponent` will search for "default" in the `Context` column of the `Translations` table.  There may be cases however where you want the same string translated differently in different contexts.  For example, if you're creating an app for fishermen who are also musicians, the word "Bass" might be referring to the fish, or to the instrument.  Providing the second argument helps disambiguate the context:

* `T.__("Bass", "Fish")` might return "loup de mer" for French, while
* `T.__("Bass", "Music")` might return "contrebasse"

You'll probably find you rarely need to use context.

### __Choices(Group) returns Table

This function gets used for LookUp columns in your app.  SharePoint in particular, but Canvas Apps more generally, do not provide a nice solution for translating choices in a Choice column.  The `TranslationComponent` provides a way to do it by making choice columns LookUp columns instead.

By putting the choices into a separate list, we can maintain the translations there.  The recommended setup is to create a single list called Choices ( using the schema described above ), and put the choices for ALL such lookup columns into that single list, using the Group column to say which choices belong together.  If your choices have an "Other" choice, mark that choice as `IsOther` in the `Choices` list.  It will be sorted to the bottom of the list for a nicer user experience.

The table that is returned from this function is suitable for use in a dropdown.  Each record contains the following properties:

* `Id` - the ID of the row in the `Choices` list
* `Value` - the human-readable choice ( in the proper language )
* `IsOther` - whether the choice is "Other".  this can be useful if you need to show a text field to allow the user to fill in something describing the "Other".
* `Title` - the original language value for this choice

This function would get used in dropdowns and can be used for fields that are either Lookup or Choices fields.

#### Lookup Fields

If you're building your schema from scratch, it's possible to choose to make choice-like fields actually be Lookup fields into a single Choices list where you can group choices together and have all of the translations in a single place.

However, as often as not, you're going to find yourself doing translation work with an existing schema that already has some lookup columns to existing lists.  That's fine.  You'll need to add a text column to those lists to hold the translated title for each record.  For every such lookup field that exists in your app, you're going to need to insert another instance of the `TranslationComponent`.  We recommend giving it a name relevant to the list it will be referencing.

Let's say you have an app that allows updates to records in a People list.  One of the fields is a Lookup field into a Departments list.  In this example, you would add the Departments list as a data source to your app, insert another instance of the `TranslationComponent` into your app and set the input parameters as such:

* `Language` - set to a variable holding the current language code of your app
* `LanguageSchema` - define a schema as described above
* `Choices` - the Departments data source
* `Translations` - you can probably leave this one blank since you'll use the more general `T` component for translating strings

Rename this inserted component to something like `T_Departments`.

Now, for the dropdown field corresponding to the Departments lookup column:

* Set `Items` to `T_Departments.__Choices()`
* Set `DefaultSelectedItems` to `Filter( T_Departments.__Choices(), Id = ThisItem.Department.Id)`

The `DefaultSelectedItems` property is necessary to set because the default `[Parent.Default]` will only ever show the default language's choice.

> You may notice that we're calling the same function twice.  As developers, we don't like to have to repeat calls to the same function.  You might consider using a User Defined function to fetch and organize all of your choices whenever the language property changes.  That's beyond the scope of this README though.  Only consider doing this if you start noticing performance is suffering in your app.

The `__Choices` function would also be used if you want to show the translated choice somewhere statically for a record, say perhaps in a Vertical Gallery.   For some text input you could use:

* `LookUp( T_Departments.__Choices(), Id = ThisItem.Department.Id).Value`

#### Choices Fields

Probably a more likely scenario is that you have a bunch of columns on your list that are Choices.  To get these translated, you'll build out a list that will hold the translations for _all_ of your Choices columns.  This translated choices list will have columns:

* `Title` - will hold the original language value of the choice
* `Group` - will hold the group string used to say which choices belong together. We recommend using the logical column name.
* Some other text field(s) that will hold the translation(s) of the choice

> Please note, there's not really a way to sync the list of choices from your column and this separate list of choices in the translated choices list.  You'll have to keep them in sync manually.  Further, the order that choices come back from the `TranslationComponent` area always alphabetical, so they might not match the order of choices in your column

With everything in place, you can update your app.  For forms, for every dropdown that corresponds to a Choices column:

* Set `Items` to `T.__Choices("MyGroup")`
* Set `DefaultSelectedItems` to `Filter( T.__Choices("MyGroup"), Title = ThisItem.MyGroup.Value)`
* Set the `Update` property of the parent DataCard to `{Value: DataCardValue1.Selected.Title}`

The `Update` property must be changed so that it's the original language choice that gets saved to the list.  That keeps things consistent.

If you want to show the translated choice somewhere, say a Vertical Gallery, you would use:

* `LookUp( T.__Choices("MyGroup"), Title = ThisItem.MyGroup.Title).Value`

Notice the subtle difference between how Lookup and Choices columns are handled.  For Lookups, the canonical value that gets saved and referenced is the Id while for Choices, the canonical value is the Title ( which is the original language choice ).


