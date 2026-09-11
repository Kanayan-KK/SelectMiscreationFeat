# SelectMiscreationFeat

Elin Mod

## Version

1.0.2

## Config

- EnableMod: Enables the functionality of this mod.
- Enable Random: Uses a random seed for gene generation.
- Choice Count: Number of gene choices to display.
- Window Width: Width of the selection list.

## Changes

- Added live partial-match search across gene names, abilities, feats, and values in the generated choices.
- Positioned the search field using the list viewport so it does not overlap the window title.
- Added EnableMod config.
- Fixed the UPGRADE achievement not unlocking after the modded miscreation upgrade flow.

[SteamWorkShop](https://steamcommunity.com/sharedfiles/filedetails/?id=3631149377)

## Search

- Type in the search box above the gene list. Results update after a 0.2-second pause.
- Matching ignores letter case. Clear the input to show all generated choices again.
- Japanese and Chinese IME composition delays filtering until conversion finishes.
- The counter shows matching / total choices, including zero matches.
- Searching preserves generated genes and their order. Each selection starts a new list with an empty search.

## Verification

- Build: `dotnet build .\SelectMiscreationFeat.sln -c Release` with `ElinGamePath` configured.
- Automated search checks (.NET 10 SDK): `dotnet run --project .\tests\GeneSearchQuery.Tests.csproj -c Release`.
- In game: check partial matches by gene name, ability, feat, and value; clearing input; zero matches; Japanese conversion; and typing candidate shortcut keys without selecting a gene.
- Check that the search field is above the visible candidates without overlapping the title or first row, including after filtering to zero results and clearing the input.
- With Choice Count set to 1000, check typing responsiveness and scrolling. Select a filtered gene and confirm its effects, the next selection, and the final UPGRADE achievement.

### Language verification

- Automated checks use representative English, Simplified Chinese, and Traditional Chinese labels under `en-US`, `zh-CN`, and `zh-TW` cultures. They cover gene names, abilities, feats, numbers, result order, zero matches, and clearing. English checks mix letter case.
- Chinese IME state checks cover waiting during composition, applying committed characters after 0.2 seconds, and cancelling composition while preserving the previous query.
- These fixtures are not official translations. Automated checks exercise search logic, not the game's language settings, fonts, or OS IME integration.
- In game, repeat the verification above with English and Chinese selected in the language settings. Search using fragments copied from the actual displayed labels. Check Chinese glyphs in both the input and results, and English labels containing spaces with mixed-case input.
- With a Chinese IME, enter and convert pinyin, confirm that composition does not refresh results, then check the committed text filters after a short pause. Cancel a conversion with Escape and confirm the selection window stays open. Check Enter and letter keys do not select genes while typing.
- Confirm the count, clearing a zero-result search, scrolling with 1000 choices, and selecting a filtered gene in each language. Game-language and rendering checks require a manual game session and are not covered by the automated test result.
