# 品質の羅針盤（QUALITY.md）

<!-- OpenTouryoCodingAgentAssets の src/compass/QUALITY.sample.md から埋めた（OpenTouryoCodingAgentAssets#21。2026-11-15）。
     このリポジトリは CI と配布物の実地の試験場で、製品のコードは src/OrderCalc（受注の金額の計算）だけである。 -->

**どこを・何で・どれだけ確かめるか**を、このリポジトリについて決めた文書。人が決め、エージェントが従う。

- **共通の決まりは [`QUALITY.common.md`](QUALITY.common.md)**（配布物。直さない）。**節の番号は同じ**で、本書には各節のこのリポジトリの中身だけを書く
- **エージェントは、変更の前に本書と `QUALITY.common.md` を読む**

> **人は個々の変更を細かく見るより、羅針盤を決める役を担う。**
> エージェントは、作業が品質マップのどの単位に当たるかを本書で引き、その重みで確かめ、報告する。

## このリポジトリの値

| 項目 | 値 |
|---|---|
| 検出器の置き場所 | `tools/ExpectationGuard` |
| 判定の用例の置き場所 | `tools/ExpectationGuard/ExpectationGuard.Tests` |
| 入れ直しのコマンド | `<アセット>\install\install.ps1 -Product claude -TargetRoot <このリポジトリ> -IncludeQualityGuard` |
| 宛先のブランチ（段 3 で検査する PR の宛先） | `main`（`.github/workflows/expectation-change.yml`） |
| 必須のチェック | まだ（`main` は保護していない） |
| 共有の作業ブランチ | 無し |
| 変更のレビュー役（4 節） | `opentouryo-change-reviewer` |
| テスト先行のレビュー役 | `opentouryo-test-reviewer`（[`TESTING.md`](TESTING.md) 5） |
| 回帰テストの名前 | テストのメソッド名に Issue の番号を入れる（例 `LineAmount_割引率を掛ける_Issue12`） |

### 一次情報

| 内容 | 一次情報 |
|---|---|
| 検証の手順 | `dotnet test tests/OrderCalc.Tests/OrderCalc.Tests.csproj` |
| 仕様（根拠の出典） | NorthwindDbSampleSpec（<https://github.com/OpenTouryoProject/NorthwindDbSampleSpec>）の `HLD/Common.md`「金額の算出式」 |

---

## 1. 品質マップ

**単位**：プロジェクト。

| 単位 | リスク | 正の根拠 | 検証の手段 | 人が必ず見る箇所 |
|---|---|---|---|---|
| `src/OrderCalc`（受注の金額の計算） | **高** | `HLD/Common.md`「金額の算出式」 | 単体テスト（NUnit。`tests/OrderCalc.Tests`） | 算出式、丸め（小数第 3 位の四捨五入）、10 進数型で計算しているか（`Discount` は単精度） |
| `tests/SampleTests.cs`・`A.cs`・`B.cs` | 低 | — | — | 原則として見ない（CI の試験用） |
| `tools/`（検出器の写し） | 低 | — | 判定の用例 | 写しは直さない（`QUALITY.common.md` 3 節） |

---

## 2. 正の定義

- **根拠**：`HLD/Common.md` の節（「金額の算出式」）か、Issue の番号
- **観測の扱い**：使わない
- **外部の基準**：無し

---

## 3. 期待値の変更と、その承認

- **期待値の変更に当たるもの**：検出器の `tools/ExpectationGuard/rules.psd1`（`tests/` の下の `.cs` で、テストの削除・`Ignore` の追加・`[Basis(` の削除・`Assert.` の削除）

---

## 4. レビューの観点

### 金額の計算（`src/OrderCalc`）

- [ ] 算出式が `HLD/Common.md`「金額の算出式」のとおりか（明細単価（割引後）→ 明細金額 → 小計 → 受注合計）
- [ ] 丸めの位置（明細ごとに丸めてから合算する）
- [ ] 浮動小数点型で計算していないか

---

## 5. フィードバックの輪

- `QUALITY.common.md` 5 節のとおり

---

## 6. 計測

- `QUALITY.common.md` 6 節のとおり（このリポジトリ固有の指標は無い）

---

## 7. 期待値の根拠の書き方

- **書き方**：テストのクラスかメソッドのコメントに、根拠（`HLD/Common.md`「金額の算出式」か Issue の番号）を書く

---

## 8. エージェント向けチェックリスト

共通の項目は `QUALITY.common.md` 8 節。このリポジトリ固有の項目は無い。
